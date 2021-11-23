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

## Steps for creating a new instance

1. Create a `hosts_myinstance` file from `hosts_example` and change values
   * `instance_name` should only contain alphanumeric characters
   * Change `domain` to the desired domain
   * Change db user name
   * Change admin user name, password, email
2. Execute `basic_setup.yml` once on this server (only needed for the first instance)
```
ansible-playbook -i hosts_myinstance basic_setup.yml
```
3. Execute `setup_wordpress.yml` once on this server
```
ansible-playbook -i hosts_myinstance setup_wordpress.yml
```
4. ... to be continued