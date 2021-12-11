<?php
/**
 * Class definition for Explorer_Event_Renderer.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Defines how an event is rendered in the explorer view.
 */
class Admin_View_Event_Renderer extends Comcal_Default_Event_Renderer {
    public function render( Comcal_Event $event, int $day ) : string {
        if ( 0 !== $day ) {
            // Render multi-day events only once.
            return '';
        }
        $pretty = new Pretty_Event( $event );

        $featherlight_view_data = Event_Popup::get_featherlight_attribute( $event );
        $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $event );

        $edit_link = $this->get_edit_link( $event );
        if ( '' !== $edit_link ) {
            $edit_link = ", <a href='#' $featherlight_edit_data>bearbeiten</a>";
        }

        $private = $event->get_field( 'public' ) ? 'public' : 'private';

        $title = wp_trim_words( $pretty->title, 10, '...' );

        $submitter = $event->get_field( 'submitterName' );
        $created   = new DateTime( $event->get_field( 'created' ) );
        $created_f = $created->format( 'd.m.Y H:i' );
        $obsolete  = $event->get_end_date_time()->is_before( Comcal_Date_Time::now() ) ? 'obsolete' : '';

        $date = $pretty->formatted_date;
        $time = $pretty->formatted_time;

        if ( '' !== $time ) {
            $time = ', ' . $time;
        }

        $location = $pretty->location;
        if ( '' !== $time && '' !== $location ) {
            $location = ', ' . $location;
        }

        $organizer = $pretty->organizer;
        if ( ( '' !== $time || '' !== $location ) && '' !== $organizer ) {
            $organizer = ', ' . $organizer;
        }

        return <<<XML
        <article class="$private $obsolete">
            <h3><a href="#" $featherlight_view_data>$title</a></h3>
            <section class="meta">
            $date$time$location$organizer$edit_link &mdash; $submitter ($created_f)
            </section>
        </article>
XML;
    }
}

Event_Popup::verify_popup_initialized();
