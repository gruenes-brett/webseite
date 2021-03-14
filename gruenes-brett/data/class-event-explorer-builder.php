<?php
/**
 * Defines how the event explorer view is rendered.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

define( 'STYLE_NAME', 'gruenes-brett-table ' );

/**
 * Creates calendar tables for each month.
 */
class Event_Explorer_Builder extends comcal_DefaultDisplayBuilder {

    /**
     * Singleton instance of this class.
     *
     * @var $instance Singleton instance of this class.
     */
    private static ?Event_Explorer_Builder $instance = null;

    /**
     * Instantiates the Event_Explorer_Builder singleton and loads the
     * events from the database.
     */
    public static function get_instance() {
        if ( null !== static::$instance ) {
            return static::$instance;
        }

        $category      = null;
        $calendar_name = 'gruenes-brett';
        $start_date    = null;
        $latest_date   = null;
        $is_admin      = comcal_current_user_can_set_public();

        $events_iterator = new comcal_EventIterator(
            ! $is_admin,
            $category,
            $calendar_name,
            $start_date ? $start_date->get_date_str() : null,
            $latest_date ? $latest_date->get_date_str() : null
        );

        static::$instance = self::create_display( static::class, $events_iterator );
        return static::$instance;
    }

    protected function __construct( $earliest_date = null, $latest_date = null ) {
        parent::__construct( $earliest_date, $latest_date );
        $this->event_renderer = new Explorer_Event_Renderer();
    }
}
