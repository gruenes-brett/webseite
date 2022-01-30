<?php
/**
 * A sitemap provider for the events.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * A sitemap provider for the events.
 */
class Event_Sitemap_Provider extends WP_Sitemaps_Provider {

    public function __construct() {
        $this->name        = 'events';
        $this->object_type = 'event';
    }

    public function get_url_list( $page_num, $object_subtype = '' ) {
        $url_list = array();

        $length = wp_sitemaps_get_max_urls( $this->object_type );
        $offset = ( $page_num - 1 ) * $length;
        $events = Comcal_Query_Event_Rows::query_events_by_date( null, '', Comcal_Date_Time::now() );
        $sliced = array_slice( $events, $offset, $length );

        foreach ( $sliced as $event ) {
            $sitemap_entry = array(
                // phpcs:ignore
                'loc' => Common_Data::get_permalink( $event->eventId ),
            );

            $url_list[] = $sitemap_entry;
        }
        return $url_list;
    }

    public function get_max_num_pages( $object_subtype = '' ) {
        $events = Comcal_Query_Event_Rows::query_events_by_date( null, '', Comcal_Date_Time::now() );
        return (int) ceil( count( $events ) / wp_sitemaps_get_max_urls( $this->object_type ) );
    }
}
