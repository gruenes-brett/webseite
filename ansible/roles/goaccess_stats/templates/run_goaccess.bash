#!/bin/bash
# Analyze the log for the current month (or yesterday's month if today is the 1st)
#
# This script copies yesterday's apache logfile to a monthly cache folder that contains all.
# While copying, the log file is already filtered for the requested domain.
# Afterwards, goaccess is called for all the logs of the month and a static html file
# is created.

DOMAIN={{ domain }}

LOGDIR=/var/log/apache2
LOGFILE_BASE=$LOGDIR/other_vhosts_access.log
OUTDIR_BASE={{ stats_base_dir }}/$DOMAIN

# consider yesterday's logs
YEAR=$(date --date="yesterday" +%Y)
MONTH=$(date --date="yesterday" +%m)
DAY=$(date --date="yesterday" +%d)

HTML_OUT_DIR=$OUTDIR_BASE/$YEAR
LOG_CACHE_DIR={{ log_cache_dir }}/$DOMAIN/$YEAR/$MONTH

mkdir -p $HTML_OUT_DIR
mkdir -p $LOG_CACHE_DIR

# Copy yesterdays log file to cache dir
cat $LOGFILE_BASE.1 | grep "^$DOMAIN" > $LOG_CACHE_DIR/access_$DAY.log

# Additional expressions for lines that should be ignored in the log file
FILTER_EXPRESSIONS='wp-cron\.php|wp-login\.php|ical\/all|ical\/ev|xmlrpc\.php|wp-json|wp-content|wp-includes|wp-admin|wp-sitemap'
FILTER_EXPRESSIONS=$FILTER_EXPRESSIONS'|Uptime-Kuma|SemrushBot|\?author='

# Create HTML for given month
zcat --force $LOG_CACHE_DIR/*.log | grep -v -E -e $FILTER_EXPRESSIONS | goaccess --output=$HTML_OUT_DIR/$MONTH.html --log-format=VCOMBINED --ignore-crawlers --no-progress -
