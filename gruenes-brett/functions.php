<?php
/**
 * The functions
 *
 * @package GruenesBrett
 */

if ( ! defined( 'ABSPATH' ) ) {
    exit;
}

require_once ABSPATH . 'wp-admin/includes/plugin.php';

/**
 * Checks if the required Community Calendar plugin is loaded.
 *
 * @param bool $echo_message set true to echo a message.
 */
function verify_community_calendar_loaded( $echo_message = false ) : bool {
    $loaded = is_plugin_active( 'community-calendar/community-calendar.php' );
    if ( ! $loaded && $echo_message ) {
        echo <<<XML
            <p>
                This theme requires the <a href="https://github.com/joergrs/community-calendar">
                Community Calendar plugin</a> to be installed!
            </p>
XML;
    }
    return $loaded;
}

require_once 'data/class-calendar-table-builder.php';
