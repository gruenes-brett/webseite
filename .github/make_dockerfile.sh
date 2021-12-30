#!/usr/bin/bash

# Quit on first error
set -euo pipefail

# Switch to script dir
cd "$(dirname "$0")" || exit 3

cat <<'EOF' >../Dockerfile
  FROM ubuntu:20.04

  # See https://doc.traefik.io/traefik/routing/routers/
  LABEL traefik.http.routers.testing.rule=Host(`testing`)

  COPY . /var/www/testing
  WORKDIR /var/www/testing

  EXPOSE 80
EOF

lando_section_to_bash() {
  local length
  local script

  length=$(yq eval ".services.web.$1 | length" ../.lando.yml)
  script=docker-$1.sh

  echo '#!/usr/bin/bash' >"$script"
  echo set -xeuo pipefail >>"$script"
  chmod +x "$script"

  for i in $(seq 0 $((length - 1))); do
    yq eval ".services.web.$1[$i]" ../.lando.yml | \
      sed 's/hosts_lando/hosts_myinstance/' >>"$script"
  done
  echo "$2" ".github/$script" >>../Dockerfile
}

lando_section_to_bash build_as_root RUN
cat <<EOF >>docker-build_as_root.sh
  mysql -e SHUTDOWN
  rm /etc/apache2/sites-enabled/000-default.conf
  echo ServerName testing >> /etc/apache2/apache2.conf
EOF

lando_section_to_bash run_as_root ENTRYPOINT
# shellcheck disable=SC1087
cat <<'EOF' >>docker-run_as_root.sh
  echo 127.0.0.1 testing >> /etc/hosts
  sed -i '2 i if (!empty($_SERVER["HTTP_X_FORWARDED_PROTO"]) && $_SERVER["HTTP_X_FORWARDED_PROTO"] === "https") $_SERVER["HTTPS"] = "on";' /var/www/testing/wordpress/wp-config.php
  tail /var/log/apache2/error.log -f
EOF

(
  cd ../ansible
  cp hosts_example hosts_myinstance
  sed -i 's/me@myhost.net/host ansible_connection=local/' hosts_myinstance
  sed -i 's/mywebsite.net/testing/' hosts_myinstance
  sed -i 's/http_prefix=https/http_prefix=http/' hosts_myinstance
)
