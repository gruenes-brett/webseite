# Setting up this website using Ansible

Tested on Ubuntu 20.04 LTS

## Overview

* Install Apache, MariaDB, PHP
* Download and extract latest WordPress
* Create and initialize database
* Create required pages
* Create Apache virtual host for the website
* Initialize Let's Encrypt certificate (TODO)

See file `hosts_example` for required and optional settings.

A server can host multiple website instances. Simply use different `instance_name` and `domain`
values in each host file.

## Prerequisites

* Host must be reachable via SSH
  * `sudo apt install openssh-server`
  * Make sure that you can login via SSH from you machine
* Install Ansible locally (e.g. `sudo apt install ansible`)

## Steps for creating a new instance (first time setup)

### 1. Create a `hosts_myinstance` file from `hosts_example` and change values
* `instance_name` should only contain alphanumeric characters (no umlauts, special characters, spaces etc.)
* Change `domain` to the desired domain
* Change `domain_alias` in case the site shall be reachable under an alternate domain (WordPress will
  automatically forward this to the main domain, though). Multiple aliases may be comma separated (without spaces!).
* Change db user name
* Change admin user name, password, email

### 2. Execute `basic_setup.yml` once on this server (only needed for the first instance)
```
ansible-playbook -i hosts_myinstance basic_setup.yml
```

### 3. Execute `setup_wordpress.yml` once for every new instance
```
ansible-playbook -i hosts_myinstance setup_wordpress.yml
```
* Initializes the database for this instance and creates database user
* Downloads latest WordPress
* Creates Apache virtual host for the page, based on the domain
* Pulls and activate gruenes-brett theme and community-calendar plugin from Github
* Creates wp-config.php and sets all necessary settings
* Sets up SMTP configuration, if `email_address` is not empty (all other `email_*`
  fields should be filled out, too). SMTP is disabled if `email_address` is empty.
* Creates all necessary pages; removes unnecessary default pages
* Sets the permalink settings
* Sets up a cron job for nightly backups of the WordPress database of this instance.
  Target dir: `/var/www/gb_backups/<instance_name>/db_<timestamp>.sql`.
  Another cron job is created that deletes backups that are older than 7 days.

### 4. For initializing or renewing the Let's Encrypt certificate, execute
```
ansible-playbook -i hosts_myinstance update_letsencrypt.yml
```

This is required if `http_prefix` is set to `https` in the host config file.

Normally this only has to be done once because the certbot automatically renews the certificate
when it's due.

### 5. Install Event Scraper service (only needed once for all instances)
```
ansible-playbook -i hosts_myinstance install_eventscraper.yml
```

This creates a service at http://127.0.0.1:5050 that can be used for
scraping event data from Facebook.

### 6. Installing the landing page
A landing page, as implemented in the 'landing_page' folder of the gruenes-brett repo,
can be installed. This playbook will also try to install Let's Encrypt certificates
for all supplied domains.

1. In `group_vars/globals`, adjust the `landing_page_domain` and `landing_page_domain_alias`
   (the latter may be empty, or even a comma separated list of multiple domains without spaces).
2. Execute the playbook
```
ansible-playbook -i hosts_myinstance setup_landing_page.yml
```

### 7. Install and use GoAccess for user access statistics
If desired, GoAccess (https://goaccess.io) may be used for analyzing user access. The access stats
will be updated every day after midnight. Monthly data is gathered into a single dashboard.
```
ansible-playbook -i hosts_myinstance setup_goaccess.yml
```

The web dashboard is accessible via http://yourdomain.com/stats/
(https is currently not supported). For security reasons, access to the stats page requires authentication.
For that, a `.htpasswd` file with credentials needs to be created manually in `/var/www/goaccess`:
```
cd /var/www/goaccess
sudo htpasswd -c .htpasswd myusername
```

### Further manual setup steps
* Creating user accounts for contributors, authors and editors
* Fill empty pages with content

## Updating to the latest theme/plugin

Run this every time the theme and plugin should be updated to the latest version:
```
ansible-playbook -i hosts_myinstance update_theme.yml
```
**Caution**: This will overwrite any manual changes that have been done to the theme or
plugin in the current instance!


## Updating to latest WordPress version
```
ansible-playbook -i hosts_myinstance update_wordpress.yml
```

## Removing a site

To remove a site, run the following playbook and confirm with Enter.

```
ansible-playbook -i hosts_myinstance remove_instance.yml
```

This deletes the page and the associated database. This cannot be undone!
