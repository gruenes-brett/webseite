<?php

if ( ! verifyCommunityCalendarLoaded() ) {
    return;
}

define('STYLE_NAME', 'gruenes-brett-table');

class gb_CalendarTableBuilder extends comcal_TableBuilder {

    static function show() {

        $category = null;
        $calendarName = 'gruenes-brett';
        $startDate = null;
        $latestDate = null;

        $isAdmin = comcal_currentUserCanSetPublic();
        $eventsIterator = new comcal_EventIterator(
            !$isAdmin,
            $category,
            $calendarName,
            $startDate ? $startDate->getDateStr() : null,
            $latestDate ? $latestDate->getDateStr() : null
        );

        $builder = self::createDisplay(STYLE_NAME, $eventsIterator);
        echo $builder->getHtml();
    }

    function __construct($earliestDate=null, $latestDate=null) {
        parent::__construct($earliestDate, $latestDate);
        $this->eventRenderer = new gb_EventRenderer();
    }

    protected function getTableHead($monthTitle) {
        return "<h2 class='month-title'>$monthTitle</h2>\n"
               . "<table><tbody>\n";
    }

    protected function createDayRow($dateTime, $text, $isNewDay=true) {
        if ( $isNewDay ) {
            $weekday = $dateTime->getShortWeekday();
            $dayOfMonth = $dateTime->getDayOfMonth();
        } else {
            $weekday = '';
            $dayOfMonth = '';
        }
        $trClass = $isNewDay ? '' : 'sameDay';
        $dateClass = ($text==='') ? 'has-no-events' : 'has-events';
        $this->html .= "<tr class='{$dateTime->getDayClasses()} $trClass day'>";
        $this->html .= "<td class='date $dateClass'>$weekday</td>";
        $this->html .= "<td class='date $dateClass'>$dayOfMonth</td>";
        $this->html .= "<td class='event'>$text</td></tr>\n";
        $this->currentDate = $dateTime;
    }

}
comcal_EventsDisplayBuilder::addStyle(STYLE_NAME, 'gb_CalendarTableBuilder');


class gb_EventRenderer extends comcal_EventRenderer {
    function render(comcal_Event $event) : string {
        $title = $event->getField('title');
        $time = $event->getDateTime()->getPrettyTime();
        $location = $event->getField('location');
        $url = $event->getField('url');
        $editLink = $this->getEditLink($event);
        return <<<XML
      <article>
        <h2><a href="$url" target="_blank">$title</a></h2>
        <section class="meta">
          $editLink &mdash; $time, $location
        </section>
      </article>
XML;
    }
}