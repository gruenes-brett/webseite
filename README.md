<p align="center">
  <img width="300" height="300" src="assets/logo-text-light.png">
</p>

Die Webseite basiert auf WordPress mit einigen Plugins. Zukünftig sollen das eigene Theme und eventuell notwendige Anpassungen an den Plugins hier veröffentlicht werden. Mitarbeit ist sehr willkommen, allerdings muss dazu hier zunächste eine gewisse Grundlage geschaffen werden. Wir bitten um Geduld.

## Usage

### Dependency

This theme relies on the functionality of the
[Community Calendar Wordpress Plugin](https://github.com/joergrs/community-calendar).
Please install the latest version of this plugin first and activate it before using
the theme.

### Initial setup steps

This theme defines a couple of page templates that implement the main functionality. In your
Wordpress setup you need to create the following pages:

| Template | Suggested page name | Required path name* | Remarks |
| --- | --- | --- | --- |
| calendar.php | Kalender | /kalender/ | |
| explore.php | Erkunden | /erkunden/ | |
| addevent.php | Veranstaltung eintragen | /veranstaltung-eintragen/ | |
| event.php | Veranstaltung | /veranstaltung/ | Shows details of an event on a separate page. As of now, the URL Slug of this page must be 'veranstaltung'. Otherwise the generated event link will not work. |
| categories.php | Kategorien bearbeiten | /kategorien-bearbeiten/ | Should be visible to admin users only. |

*) During development, many links are still hard coded to these path names.
