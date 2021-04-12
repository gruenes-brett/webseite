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
class Calendar_Table_Builder extends Comcal_Table_Builder {

    /**
     * Singleton instance of this class.
     *
     * @var $instance Singleton instance of this class.
     */
    private static ?Calendar_Table_Builder $instance = null;

    /**
     * Instantiates the Calendar_Table_Builder singleton and loads the
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
        $is_admin      = user_can_administer_events();

        $events_iterator = new Comcal_Event_Iterator(
            ! $is_admin,
            $category,
            $calendar_name,
            $start_date ? $start_date->get_date_str() : null,
            $latest_date ? $latest_date->get_date_str() : null
        );

        static::$instance = self::create_display( static::class, $events_iterator );
        return static::$instance;
    }

    /**
     * List of month names and links in the tables.
     *
     * @var $month_links array( month_title => month_link )
     */
    private $month_links = array();

    /**
     * Initializes the renderer.
     *
     * @param Comcal_Date_Time $earliest_date Start date to load from database.
     * @param Comcal_Date_Time $latest_date Last date to load from database.
     */
    protected function __construct( $earliest_date = null, $latest_date = null ) {
        parent::__construct( $earliest_date, $latest_date );
        $this->event_renderer = new Table_Event_Renderer();
        $this->month_links    = array();
    }

    public function get_table_html() {
        return $this->get_html();
    }

    public function get_month_links() {
        return $this->month_links;
    }

    protected function get_table_head( $date ) {
        $month_title = $date->get_month_title();
        $month_link  = $date->get_month_link();

        $this->month_links[ $month_title ] = $month_link;
        return "<table><tbody><tr><td><a href='#$month_link' name='$month_link'>$month_title</a></td><td></td><td></td><td></td>\n";
    }

    protected function create_day_row( $date_time, $text, $is_new_day = true ) {
        if ( $is_new_day ) {
            $weekday      = $date_time->get_short_weekday();
            $day_of_month = $date_time->get_day_of_month();
        } else {
            $weekday      = '';
            $day_of_month = '';
        }
        $tr_class   = $is_new_day ? '' : 'sameDay';
        $date_class = ( '' === $text ) ? 'has-no-events' : 'has-events';

        $this->html .= "<tr class='{$date_time->get_day_classes()} $tr_class day'><td></td>";
        $this->html .= "<td class='date $date_class'>$day_of_month</td>";
        $this->html .= "<td class='date $date_class'>$weekday</td>";
        $this->html .= "<td class='event'>$text</td></tr>\n";

        $this->current_date = $date_time;
    }

}
