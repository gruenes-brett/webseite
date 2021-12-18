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
    protected function initialize_prettier_map() {
        $map = parent::initialize_prettier_map();

        // Useless example!
        $map['upperTitle'] = function () {
            return strtoupper( $this->title );
        };

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

        return $map;
    }
}
