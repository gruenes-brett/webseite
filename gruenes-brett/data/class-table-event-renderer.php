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
     */
    public function render( Comcal_Event $event ) : string {
        $title    = $event->get_field( 'title' );
        $time     = $event->get_start_date_time()->get_pretty_time();
        $location = $event->get_field( 'location' );

        $edit_link = $this->get_edit_link( $event );

        $featherlight_data = Event_Popup::get_featherlight_attribute( $event );

        return <<<XML
      <article>
        <h2><a href="#" $featherlight_data>$title</a></h2>
        <section class="meta">
          $edit_link $time, $location
        </section>
      </article>
XML;
    }
}

Event_Popup::verify_popup_initialized();