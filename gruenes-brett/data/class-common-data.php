<?php
/**
 * Common helper functions for handling event and category data.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Helper class that provides function for getting events, categories etc.
 */
class Common_Data {
    /**
     * Determine category as specified by the Get parameters.
     */
    public static function get_active_category() : ?Comcal_Category {
        $category = null;
        if ( isset( $_GET['category'] ) ) {
            $category = Comcal_Category::query_from_name(
                sanitize_text_field( wp_unslash( $_GET['category'] ) )
            );
        }
        return $category;
    }

    /**
     * Determine category name as specified by the Get parameters.
     */
    public static function get_active_category_name() : string {
        $active_category = static::get_active_category();
        return null !== $active_category ? $active_category->get_field( 'name' ) : '';
    }

    /**
     * Create an Event iterator.
     */
    public static function get_events_iterator() : Comcal_Event_Iterator {
        $category      = static::get_active_category();
        $calendar_name = '';
        $latest_date   = null;
        $start_date    = Comcal_Date_Time::now();

        $events_iterator = Comcal_Event_Iterator::load_from_database(
            $category,
            $calendar_name,
            $start_date ? $start_date->get_date_str() : null,
            $latest_date ? $latest_date->get_date_str() : null
        );
        return $events_iterator;
    }

    /**
     * Create an Event iterator for past events.
     */
    public static function get_past_events_iterator() : Comcal_Event_Iterator {
        $category      = static::get_active_category();
        $calendar_name = '';
        $latest_date   = Comcal_Date_Time::now();
        $start_date    = null;

        $events_iterator = Comcal_Event_Iterator::load_from_database(
            $category,
            $calendar_name,
            $start_date ? $start_date->get_date_str() : null,
            $latest_date ? $latest_date->get_date_str() : null
        );
        return $events_iterator;
    }

    /**
     * Returns the permalink for a given event_id.
     */
    public static function get_permalink( $event_id ) : string {
        return esc_url( get_home_url() . '/veranstaltung/' . $event_id );
    }

    /**
     * Returns the beginning of an iCal calender.
     */
    public static function get_ical_calendar_begin() : string {
        return 'BEGIN:VCALENDAR' . PHP_EOL
             . 'VERSION:2.0' . PHP_EOL
             . 'PRODID:-//gruenesbrett//NONSGML v1.0//EN' . PHP_EOL
             . 'NAME:' . get_bloginfo( 'name' ) . PHP_EOL
             . 'X-WR-CALNAME:' . get_bloginfo( 'name' ) . PHP_EOL
             . 'X-WR-TIMEZONE:Europe/Berlin' . PHP_EOL;
    }

    /**
     * Returns the end of an iCal calender.
     */
    public static function get_ical_calendar_end() : string {
        return 'END:VCALENDAR';
    }
}
