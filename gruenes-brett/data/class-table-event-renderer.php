<?php
/**
 * Class definition for Table_Event_Renderer.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Defines how an event is rendered in the calendar table.
 */
class Table_Event_Renderer extends Comcal_Event_Renderer {
    /**
     * Creates the HTML for an event.
     *
     * @param Comcal_Event $event Event instance.
     * @param int          $day This is the n-th day instance of this event (starting at 0).
     *                     Check $event->get_number_of_days() for total amount of days.
     */
    public function render( Comcal_Event $event, int $day ) : string {
        $pretty = new Pretty_Event( $event );

        $featherlight_view_data = Event_Popup::get_featherlight_attribute( $event );
        $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $event );

        $edit_link = $this->get_edit_link( $event );
        if ( '' !== $edit_link ) {
            $edit_link = ", <a href='#' $featherlight_edit_data>bearbeiten</a>";
        }

        $private = $event->get_field( 'public' ) ? '' : 'private';

        $day_of_day     = '';
        $number_of_days = $event->get_number_of_days();
        $header_tag     = 'h2';
        if ( $number_of_days > 1 ) {
            $current_day = $day + 1;
            $day_of_day  = " (Tag $current_day von $number_of_days)";
            if ( 0 !== $day ) {
                $header_tag = 'h3';
            }
        }

        $time     = $pretty->pretty_time;
        $location = $pretty->location;
        if ( '' !== $time && '' !== $location ) {
            $location = ', ' . $location;
        }

        // TODO @sebastianlay: Format event on calendar page as desired.
        return <<<XML
      <article class="$private">
        <$header_tag><a href="#" $featherlight_view_data>$pretty->title$day_of_day</a></$header_tag>
        <section class="meta">
            $time$location$edit_link
        </section>
      </article>
XML;
    }
}
