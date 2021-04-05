<?php
/**
 * The functions
 *
 * @package GruenesBrett
 */

if ( ! defined( 'ABSPATH' ) ) {
    exit;
}

require_once ABSPATH . 'wp-admin/includes/plugin.php';  // needed for is_plugin_active.

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


/**
 * Helper function that puts basic elements into the HTML
 * - Event popup
 * - Event edit form
 * - Floating buttons
 */
function echo_buttons_and_forms() {
    echo comcal_get_edit_form( 'gruenes-brett' );
    if ( comcal_current_user_can_set_public() ) {
        echo comcal_get_edit_categories_dialog();
    }
    echo comcal_floating_buttons_func( array( 'addEvent' => true ) );
}

require_once 'data/class-event-popup.php';
require_once 'data/class-edit-event-popup.php';
require_once 'data/class-calendar-table-builder.php';
require_once 'data/class-table-event-renderer.php';
require_once 'data/class-event-explorer-builder.php';
require_once 'data/class-explorer-event-renderer.php';
require_once 'data/class-pretty-event.php';
require_once 'data/class-category-provider.php';
