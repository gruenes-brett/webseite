<?php
/**
 * Defines how the calendar table is rendered.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

define( 'STYLE_NAME', 'gruenes-brett-table' );

/**
 * Creates calendar tables for each month.
 */
class Calendar_Table_Builder extends comcal_TableBuilder {

    /**
     * Helper function that loads the events from the database and returns the calendar tables.
     *
     * @return string HTML string.
     */
    public static function create_tables() : string {

        $category      = null;
        $calendar_name = 'gruenes-brett';
        $start_date    = null;
        $latest_date   = null;
        $is_admin      = comcal_currentUserCanSetPublic();

        $events_iterator = new comcal_EventIterator(
            ! $is_admin,
            $category,
            $calendar_name,
            $start_date ? $start_date->getDateStr() : null,
            $latest_date ? $latest_date->getDateStr() : null
        );

        $builder = self::createDisplay( STYLE_NAME, $events_iterator );
        return $builder->getHtml();
    }

    /**
     * Initializes the renderer.
     *
     * @param comcal_DateTime $earliest_date Start date to load from database.
     * @param comcal_DateTime $latest_date Last date to load from database.
     */
    public function __construct( $earliest_date = null, $latest_date = null ) {
        parent::__construct( $earliest_date, $latest_date );
        $this->event_renderer = new Table_Event_Renderer();
    }

    protected function getTableHead( $month_title ) {
        return "<h2 class='month-title'>$month_title</h2>\n"
               . "<table><tbody>\n";
    }

    protected function createDayRow( $date_time, $text, $is_new_day = true ) {
        if ( $is_new_day ) {
            $weekday      = $date_time->getShortWeekday();
            $day_of_month = $date_time->getDayOfMonth();
        } else {
            $weekday      = '';
            $day_of_month = '';
        }
        $tr_class   = $is_new_day ? '' : 'sameDay';
        $date_class = ( '' === $text ) ? 'has-no-events' : 'has-events';

        $this->html .= "<tr class='{$date_time->getDayClasses()} $tr_class day'>";
        $this->html .= "<td class='date $date_class'>$weekday</td>";
        $this->html .= "<td class='date $date_class'>$day_of_month</td>";
        $this->html .= "<td class='event'>$text</td></tr>\n";

        $this->current_date = $date_time;
    }

}
comcal_EventsDisplayBuilder::addStyle( STYLE_NAME, 'Calendar_Table_Builder' );
