<?php
/**
 * The functions
 *
 * @package GruenesBrett
 */

if ( ! defined( 'ABSPATH' ) ) {
    exit;
}

require_once( ABSPATH . 'wp-admin/includes/plugin.php' );

function verifyCommunityCalendarLoaded($echoMessage=false) : bool {
    $loaded = is_plugin_active( 'community-calendar/community-calendar.php' );
    if ( ! $loaded && $echoMessage ) {
        echo <<<XML
            <p>
                This theme requires the <a href="https://github.com/joergrs/community-calendar">
                Community Calendar plugin</a> to be installed!
            </p>
XML;
    }
    return $loaded;
}

require_once( 'data/calendar-table-builder.php' );