<p align="center">
  <img width="300" height="300" src="assets/logo-text-light.png">
</p>

Die Webseite basiert auf WordPress mit einigen Plugins. Zukünftig sollen das eigene Theme und eventuell notwendige Anpassungen an den Plugins hier veröffentlicht werden. Mitarbeit ist sehr willkommen, allerdings muss dazu hier zunächste eine gewisse Grundlage geschaffen werden. Wir bitten um Geduld.

## Usage

### Initial setup steps

For local development, you can use the tool [Lando](https://lando.dev/) to get a running Docker container with database,
plugin and theme ready to go. Here are the steps:

1. Run `lando start`
2. When lando has finished, check out the website at http://gruenes-brett.lndo.site:8889

It uses playbooks described in the [Ansible README](ansible/README.md).

### Dependency

This theme relies on the functionality of the
[Community Calendar Wordpress Plugin](https://github.com/gruenes-brett/community-calendar).
Please install the latest version of this plugin first and activate it before using
the theme.
