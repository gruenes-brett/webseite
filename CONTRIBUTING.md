# Aufsetzen einer Entwicklungsumgebung

Die Seite wird momentan mit [Wordpress](https://wordpress.org/) 5.6.2 und [PHP](https://www.php.net/downloads.php) 7.x entwickelt und getestet.

## stylint

Für das Linting von CSS-Dateien wird [stylint](https://stylelint.io/) verwendet. Für die Installation und Ausführung wird Node.js und NPM benötigt:

```
npm install
npx stylelint *.css
```

## PHP_CodeSniffer

Für das Linting von PHP-Dateien wird [PHP_CodeSniffer](https://github.com/squizlabs/PHP_CodeSniffer) verwendet. Für die Installation wird [Composer](https://getcomposer.org/) benötigt:

```
composer install
./vendor/bin/phpcs index.php
```