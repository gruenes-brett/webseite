<?php
/**
 * Defines how the calendar table is rendered.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Creates calendar tables for each month.
 */
class Past_Calendar_Table_Builder extends Calendar_Table_Builder {

    /**
     * Singleton instance of this class.
     *
     * @var $instance Singleton instance of this class.
     */
    private static ?Past_Calendar_Table_Builder $instance = null;

    /**
     * Instantiates the Past_Calendar_Table_Builder singleton and loads the
     * events from the database.
     */
    public static function get_instance() {
        if ( null !== static::$instance ) {
            return static::$instance;
        }

        $events_iterator = Common_Data::get_past_events_iterator();

        static::$instance = self::create_display(
            static::class,
            $events_iterator,
            Comcal_Date_Time::from_date_str_time_str( '2022-01-01', '00:00' ),
            Comcal_Date_Time::now()
        );
        return static::$instance;
    }
}
