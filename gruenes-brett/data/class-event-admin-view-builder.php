<?php
/**
 * Defines how the event admin view is rendered.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Creates calendar tables for each month.
 */
class Event_Admin_View_Builder extends Comcal_Default_Display_Builder {

    /**
     * Singleton instance of this class.
     *
     * @var $instance Singleton instance of this class.
     */
    private static ?Event_Admin_View_Builder $instance = null;

    /**
     * Instantiates the Event_Explorer_Builder singleton and loads the
     * events from the database.
     */
    public static function get_instance() {
        if ( null !== static::$instance ) {
            return static::$instance;
        }

        $event_rows      = Comcal_Query_Event_Rows::query_events_by_creation();
        $events_iterator = new Comcal_Event_Iterator( $event_rows );

        static::$instance = self::create_display( static::class, $events_iterator );
        return static::$instance;
    }

    protected function __construct( $earliest_date = null, $latest_date = null ) {
        parent::__construct( $earliest_date, $latest_date );
        $this->event_renderer = new Admin_View_Event_Renderer();
    }

    public function get_html() {
        $html = parent::get_html();

        return <<<XML
        <h2>Event-Historie</h2>
        $html
XML;
    }
}
