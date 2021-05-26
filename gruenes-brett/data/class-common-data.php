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
     * Create an Event iterator.
     */
    public static function get_events_iterator() : Comcal_Event_Iterator {
        $category      = static::get_active_category();
        $calendar_name = '';
        $latest_date   = null;
        $start_date    = static::get_earliest_display_date();
        $is_admin      = Comcal_User_Capabilities::administer_events();

        $events_iterator = new Comcal_Event_Iterator(
            ! $is_admin,
            $category,
            $calendar_name,
            $start_date ? $start_date->get_date_str() : null,
            $latest_date ? $latest_date->get_date_str() : null
        );
        return $events_iterator;
    }

    /**
     * When to start to show events.
     *
     * @return Comcal_Date_Time Timestamp or null for all events.
     */
    public static function get_earliest_display_date() {
        if ( Comcal_User_Capabilities::administer_events() ) {
            return null;
        }
        return Comcal_Date_Time::now();
    }
}
