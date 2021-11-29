# Setting up this website using Ansible

Tested on Ubuntu 20.04 LTS

## Overview

* Install Apache, MariaDB, PHP
* Download and extract latest WordPress
* Create and initialize database
* Create required pages
* Create Apache virtual host for the website
* Initialize Let's Encrypt certificate (TODO)

See file `hosts_example` for required settings.

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
* Change db user name
* Change admin user name, password, email

### 2. Execute `basic_setup.yml` once on this server (only needed for the first instance)
```
ansible-playbook -i hosts_myinstance basic_setup.yml
```

### 3. Execute `setup_wordpress.yml` once on this server
```
ansible-playbook -i hosts_myinstance setup_wordpress.yml
```

The following things are currently not automated yet and need to be done manually:
* Settings -> Permalinks ==> **Post name**
* Deleting unwanted pages (e.g., *Sample page*)
* Setting visibility of page "Kategorien bearbeiten" to *Private*
* Setting the homepage to Kalender (Customize -> Homepage Settings)

### 4. For initializing or renewing the Let's Encrypt certificate, execute
```
ansible-playbook -i hosts_myinstance update_letsencrypt.yml
```

This is required if `http_prefix` is set to `https` in the host config file.

Normally this only has to be done once because the certbot automatically renews the certificate
when it's due.

### 5. Install Event Scraper service (only neede once for all instances)
```
ansible-playbook -i hosts_myinstance install_eventscraper.yml
```

This creates a service at http://127.0.0.1:5050 that can be used for
scraping event data from Facebook.


### 6. Setting up SMTP for outgoing emails
This playbook install msmtp and sets it as default mail transport agent for outgoing
emails on the target machine. The current setup only allows a single SMTP config
that will be used by all running WordPress instances.

Make sure to fill out the `email_...` variables in your hosts config file.
```
ansible-playbook -i hosts_myinstance setup_email_smtp.yml
```

### Further manual setup steps
* Creating user accounts for contributors, authors and editors
* Changing language, time zone etc.

## Updating to the latest theme/plugin

Run this every time the theme and plugin should be updated to the latest version:
```
ansible-playbook -i hosts_myinstance update_theme.yml
```
**Caution**: This will overwrite any manual changes that have been done to the theme or
plugin in the current instance!


## Removing a site

To remove a site, run the following playbook

```
ansible-playbook -i hosts_myinstance update_theme.yml
```

This deletes the page and the associated database. This cannot be undone!
