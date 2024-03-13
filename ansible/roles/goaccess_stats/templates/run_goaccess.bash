#!/bin/bash
# Analyze the log for the current month (or yesterday's month if today is the 1st)
#
# This script copies yesterday's apache logfile to a monthly cache folder that contains all.
# While copying, the log file is already filtered for the requested domain.
# Afterwards, goaccess is called for all the logs of the month and a static html file
# is created.

# Optionally provide parameters YEAR and MONTH to re-run analysis for the given month
# Example: ./run_goaccess.bash 2024 03

DOMAIN={{ domain }}

LOGDIR=/var/log/apache2
LOGFILE_BASE=$LOGDIR/other_vhosts_access.log
OUTDIR_BASE={{ stats_base_dir }}/$DOMAIN

# consider yesterday's logs (if YEAR and MONTH are not given by parameter)
YEAR=${1:-$(date --date="yesterday" +%Y)}
MONTH=${2:-$(date --date="yesterday" +%m)}
DAY=${3:-$(date --date="yesterday" +%d)}

HTML_OUT_DIR=$OUTDIR_BASE/$YEAR
LOG_CACHE_DIR={{ log_cache_dir }}/$DOMAIN/$YEAR/$MONTH

mkdir -p $HTML_OUT_DIR
mkdir -p $LOG_CACHE_DIR

YESTERDAY=$(date --date="yesterday" +%Y-%m-%d)
if [ "$YEAR-$MONTH-$DAY" = "$YESTERDAY" ]; then
    echo "Copy yesterdays log file from $YESTERDAY to cache dir $LOG_CACHE_DIR"
    cat $LOGFILE_BASE.1 | grep "^$DOMAIN" > $LOG_CACHE_DIR/access_$DAY.log
else
    echo "Running in analysis only mode for $YEAR-$MONTH"
fi

# Additional expressions for lines that should be ignored in the log file
FILTER_EXPRESSIONS='wp-cron\.php|wp-login\.php|ical\/all|ical\/ev|xmlrpc\.php|wp-json|wp-content|wp-includes|wp-admin|wp-sitemap'
FILTER_EXPRESSIONS=$FILTER_EXPRESSIONS'|Uptime-Kuma|Bot|\?author='

# Create HTML for given month
zcat --force $LOG_CACHE_DIR/*.log | grep -v -E -e $FILTER_EXPRESSIONS | goaccess --output=$HTML_OUT_DIR/$MONTH.html --log-format=VCOMBINED --ignore-crawlers --no-progress -
