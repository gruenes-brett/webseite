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
        $title     = $event->getField( 'title' );
        $time      = $event->getDateTime()->getPrettyTime();
        $location  = $event->getField( 'location' );
        $url       = $event->getField( 'url' );
        $edit_link = $this->getEditLink( $event );
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
