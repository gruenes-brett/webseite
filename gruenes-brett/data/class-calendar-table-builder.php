<?php

if ( ! verify_community_calendar_loaded() ) {
    return;
}

define( 'STYLE_NAME', 'gruenes-brett-table' );

class Calendar_Table_Builder extends comcal_TableBuilder {

    public static function show() {

        $category = null;
        $calendar_name = 'gruenes-brett';
        $start_date = null;
        $latest_date = null;

        $is_admin = comcal_currentUserCanSetPublic();
        $events_iterator = new comcal_EventIterator(
            ! $is_admin,
            $category,
            $calendar_name,
            $start_date ? $start_date->getDateStr() : null,
            $latest_date ? $latest_date->getDateStr() : null
        );

        $builder = self::createDisplay( STYLE_NAME, $events_iterator );
        echo $builder->getHtml();
    }

    function __construct( $earliest_date = null, $latest_date = null ) {
        parent::__construct( $earliest_date, $latest_date );
        $this->eventRenderer = new gb_EventRenderer();
    }

    protected function getTableHead( $month_title ) {
        return "<h2 class='month-title'>$month_title</h2>\n"
               . "<table><tbody>\n";
    }

    protected function createDayRow( $date_time, $text, $is_new_day = true ) {
        if ( $is_new_day ) {
            $weekday = $date_time->getShortWeekday();
            $day_of_month = $date_time->getDayOfMonth();
        } else {
            $weekday = '';
            $day_of_month = '';
        }
        $tr_class = $is_new_day ? '' : 'sameDay';
        $date_class = ( $text === '' ) ? 'has-no-events' : 'has-events';
        $this->html .= "<tr class='{$date_time->getDayClasses()} $tr_class day'>";
        $this->html .= "<td class='date $date_class'>$weekday</td>";
        $this->html .= "<td class='date $date_class'>$day_of_month</td>";
        $this->html .= "<td class='event'>$text</td></tr>\n";
        $this->currentDate = $date_time;
    }

}
comcal_EventsDisplayBuilder::addStyle( STYLE_NAME, 'Calendar_Table_Builder' );


class Event_Renderer extends comcal_EventRenderer {
    function render( comcal_Event $event ) : string {
        $title = $event->getField( 'title' );
        $time = $event->getDateTime()->getPrettyTime();
        $location = $event->getField( 'location' );
        $url = $event->getField( 'url' );
        $edit_link = $this->getEditLink( $event );
        return <<<XML
      <article>
        <h2><a href="$url" target="_blank">$title</a></h2>
        <section class="meta">
          $edit_link &mdash; $time, $location
        </section>
      </article>
XML;
    }
}
