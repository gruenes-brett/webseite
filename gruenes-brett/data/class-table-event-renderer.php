<?php
/**
 * Class definition for Table_Event_Renderer.
 *
 * @package GruenesBrett
 */

/**
 * Defines how an event is rendered in the calendar table.
 */
class Table_Event_Renderer extends comcal_EventRenderer {
    /**
     * Creates the HTML for an event.
     *
     * @param comcal_Event $event Event instance.
     */
    public function render( comcal_Event $event ) : string {
        $title     = $event->get_field( 'title' );
        $time      = $event->get_start_date_time()->get_pretty_time();
        $location  = $event->get_field( 'location' );
        $url       = $event->get_field( 'url' );
        $edit_link = $this->get_edit_link( $event );
        return <<<XML
      <article>
        <h2><a href="$url" target="_blank">$title</a></h2>
        <section class="meta">
          $edit_link $time, $location
        </section>
      </article>
XML;
    }
}
