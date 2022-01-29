<?php
/**
 * A helper for getting human readable details out of a Comcal_Event instance.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Event prettifier.
 */
class Pretty_Event extends Comcal_Pretty_Event {

    /**
     * Comcal_Event instance.
     *
     * @var Comcal_Event
     */
    protected $event;

    public function __construct( Comcal_Event $event ) {
        parent::__construct( $event );
        $this->event = $event;
    }

    protected function initialize_prettier_map() {
        $map = parent::initialize_prettier_map();

        $map['formattedDate'] = function () {
            $start = Comcal_Date_Time::from_date_str_time_str( $this->date, $this->time );
            $end   = Comcal_Date_Time::from_date_str_time_str( $this->date_end, $this->time_end );

            $date = $start->get_short_weekday() . ' ' . $start->get_pretty_date();
            if ( $this->is_multiday ) {
                $date .= ' bis ' . $end->get_short_weekday() . ' ' . $end->get_pretty_date();
            }
            return $date;
        };

        $map['formattedTime'] = function () {
            $start = new DateTime( $this->date . 'T' . $this->time );
            $end   = new DateTime( $this->date_end . 'T' . $this->time_end );

            $time = '';
            if ( $start->format( 'H:i' ) !== '00:00' ) {
                $time .= $start->format( 'H:i' );
                if ( $this->time !== $this->time_end ) {
                    $time .= ' – ' . $end->format( 'H:i' );
                }
                $time .= ' Uhr';
            }
            return $time;
        };

        $map['is_multiday'] = function() {
            return $this->date !== $this->date_end;
        };

        $map['meta'] = function() {
            $date = $this->formatted_date;
            $time = $this->formatted_time;

            if ( '' !== $time ) {
                $time = ', ' . $time;
            }
            $location = $this->location;

            if ( '' !== $location ) {
                $location = '<br>' . $location;
            }

            $address = $this->address;
            if ( '' !== $address ) {
                $address = '<br>' . $address;
            }

            $organizer = $this->organizer;
            if ( '' !== $organizer ) {
                $organizer = '<br>' . $organizer;
            }

            return $date . $time . $location . $address . $organizer;
        };

        $map['clickable_description'] = function() {
            return make_clickable( $this->description );
        };

        $map['ics'] = function() {
            $dtstamp = $this->event->get_created_date()->format( 'Ymd\THis' );
            $dtstart = $this->event->get_start_date_time( 0 )->format( 'Ymd\THis' );
            $dtend   = $this->event->get_end_date_time()->format( 'Ymd\THis' );

            $ics_start = rawurlencode(
                'BEGIN:VCALENDAR' . PHP_EOL
                . 'VERSION:2.0' . PHP_EOL
                . 'PRODID:-//gruenesbrett//NONSGML v1.0//EN' . PHP_EOL
                . 'BEGIN:VEVENT' . PHP_EOL
                . "UID:$this->event_id@gruenesbrett" . PHP_EOL
                . "DTSTAMP:$dtstamp" . PHP_EOL
                . "DTSTART:$dtstart" . PHP_EOL
                . "DTEND:$dtend" . PHP_EOL
                . "SUMMARY:$this->title" . PHP_EOL
            );
            $ics_desc  = 'DESCRIPTION: ' . str_replace( '<br />', '\\n', $this->description );
            $ics_end   = rawurlencode( PHP_EOL . 'END:VEVENT' . PHP_EOL . 'END:VCALENDAR' );

            return $ics_start . $ics_desc . $ics_end;
        };

        $map['permalink'] = function() {
            return esc_url( get_home_url() . '/veranstaltung/' . $this->event_id );
        };

        $map['json_schema'] = function() {
            $dtstart = $this->event->get_start_date_time( 0 )->format( 'Ymd\THis' );
            $dtend   = $this->event->get_end_date_time()->format( 'Ymd\THis' );

            return <<<XML
                <script type="application/ld+json">
                {
                "@context": "https://schema.org",
                "@type": "Event",
                "location": {
                    "@type": "Place",
                    "name": "$this->location"
                },
                "name": "$this->title",
                "startDate": "$dtstart",
                "endDate": "$dtend",
                "organizer": {
                    "@type": "Organization",
                    "name": "$pretty->organizer"
                }
                }
                </script>
            XML;
        };

        $map['name'] = function() {
            return trim( $this->title );
        };

        return $map;
    }
}
