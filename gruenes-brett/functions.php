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

add_action(
    'after_setup_theme',
    function() {
        verify_community_calendar_loaded( true );
    }
);


/**
 * Helper function that puts basic elements into the HTML
 * - Floating buttons
 */
function echo_floating_buttons() {
    echo comcal_floating_buttons_func( array( 'addEvent' => false ) );
}

require_once 'data/class-common-data.php';

require_once 'data/class-event-popup.php';
require_once 'data/class-edit-event-popup.php';
require_once 'data/class-edit-event-form.php';

// calendar.
require_once 'data/class-calendar-table-builder.php';
require_once 'data/class-table-event-renderer.php';

// explorer.
require_once 'data/class-event-explorer-builder.php';
require_once 'data/class-explorer-event-renderer.php';

// helpers.
require_once 'data/class-pretty-event.php';
require_once 'data/class-category-provider.php';

// define custom media size.
add_image_size( 'gruenesbrett', 960, 540 );

/**
 * Custom API route that handles the upload to the media library.
 *
 * @param WP_REST_Request $data contains the uploaded image.
 */
function upload_image( WP_REST_Request $data ) {
    $image      = $data->get_file_params()['image'];
    $attachment = media_handle_sideload( $image, 0 );

    if ( ! is_wp_error( $attachment ) ) {
        $url = wp_get_attachment_image_url( $attachment, 'gruenesbrett' );
        return $url;
    }
}

add_action(
    'rest_api_init',
    function () {
        register_rest_route(
            'gruenesbrett/v1',
            '/upload/',
            array(
                'methods'             => 'POST',
                'callback'            => 'upload_image',
                'permission_callback' => '__return_true',
            )
        );
    }
);

/**
 * Custom rewrite rule to enable virtual event pages.
 */
add_action(
    'init',
    function() {
        add_rewrite_rule(
            '^veranstaltung/(.+)/?',
            'index.php?pagename=veranstaltung&event_id=$matches[1]',
            'top'
        );
    },
    10,
    0
);

add_action(
    'init',
    function() {
        add_rewrite_tag( '%event_id%', '([^&]+)' );
    },
    10,
    0
);

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
    wp_enqueue_script(
        'stimulus',
        esc_url( get_stylesheet_directory_uri() . '/js/stimulus.umd.js' ),
        '',
        $version,
        true
    );
    wp_enqueue_script(
        'script',
        esc_url( get_stylesheet_directory_uri() ) . '/js/script.js',
        '',
        $version,
        true
    );
    wp_localize_script(
        'script',
        'wp_api',
        array(
            'root'  => esc_url_raw( rest_url() ),
            'nonce' => wp_create_nonce( 'wp_rest' ),
        )
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
