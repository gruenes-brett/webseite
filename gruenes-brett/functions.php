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
 * Returns whether the currently logged in user may edit or delete events.
 *
 * @return bool
 */
function user_can_administer_events() : bool {
    return current_user_can( 'edit_others_posts' );
}


/**
 * Helper function that puts basic elements into the HTML
 * - Event popup
 * - Event edit form
 * - Floating buttons
 */
function echo_buttons_and_forms() {
    echo comcal_get_edit_form( 'gruenes-brett' );
    if ( user_can_administer_events() ) {
        echo comcal_get_edit_categories_dialog();
    }
    echo comcal_floating_buttons_func( array( 'addEvent' => true ) );
}

require_once 'data/class-event-popup.php';
require_once 'data/class-edit-event-popup.php';
require_once 'data/class-edit-event-form.php';
require_once 'data/class-calendar-table-builder.php';
require_once 'data/class-table-event-renderer.php';
require_once 'data/class-event-explorer-builder.php';
require_once 'data/class-explorer-event-renderer.php';
require_once 'data/class-pretty-event.php';
require_once 'data/class-category-provider.php';


/**
 * Enqueue scripts and styles.
 */
function gruenes_brett_scripts() {
    $version = '1.0';
    wp_enqueue_script(
        'gruenes_brett_form_script',
        esc_url( get_stylesheet_directory_uri() . '/js/forms.js' ),
        array( 'jquery', 'jquery-form' ),
        $version,
        true
    );
    wp_enqueue_script(
        'featherlight',
        esc_url( get_stylesheet_directory_uri() ) . '/js/featherlight.min.js',
        '',
        $version,
        true
    );
}
add_action( 'wp_enqueue_scripts', 'gruenes_brett_scripts' );

function gruenes_brett_styles() {
    $version = '1.0';
    wp_enqueue_style( 'style', get_stylesheet_uri(), '', '1.0' );
    wp_enqueue_style(
        'featherlight',
        esc_url( get_stylesheet_directory_uri() ) . '/css/featherlight.min.css',
        '',
        '1.0'
    );
}
add_action( 'wp_print_styles', 'gruenes_brett_styles' );
