<?php
/**
 * Returns the iCal
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

$ical     = get_query_var( 'ical' );
$event    = Comcal_Event::query_by_entry_id( $ical );
$category = Common_Data::get_active_category_name();

if ( null !== $event ) { // this is the iCal for a single event.

    $pretty = new Pretty_Event( $event );

    header( 'Content-Type: text/calendar;charset=utf8', true );
    header( 'Content-Disposition: attachment; filename="' . $pretty->event_id . '.ics"', true );

    echo Common_Data::get_ical_calendar_begin();
    echo htmlspecialchars_decode( $pretty->ical );
    echo Common_Data::get_ical_calendar_end();

} elseif ( ! empty( $category ) ) { // this is the iCal for a specific category.

    header( 'Content-Type: text/calendar;charset=utf8', true );
    header( 'Content-Disposition: attachment; filename="' . $category . '.ics"', true );

    echo Common_Data::get_ical_calendar_begin();
    echo htmlspecialchars_decode( Event_Ical_Builder::get_instance()->get_html() );
    echo Common_Data::get_ical_calendar_end();

} else { // this is the iCal for all events.

    header( 'Content-Type: text/calendar;charset=utf8', true );
    header( 'Content-Disposition: attachment; filename="all.ics"', true );

    echo Common_Data::get_ical_calendar_begin();
    echo htmlspecialchars_decode( Event_Ical_Builder::get_instance()->get_html() );
    echo Common_Data::get_ical_calendar_end();

}
