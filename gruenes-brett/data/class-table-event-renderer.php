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
        $pretty = new Pretty_Event( $event );

        $edit_link = $this->get_edit_link( $event );

        $featherlight_view_data = Event_Popup::get_featherlight_attribute( $event );
        $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $event );

        $edit_link_popup = '';
        if ( '' !== $edit_link ) {
            $edit_link_popup = "<a href='#' $featherlight_edit_data>editfs</a>";
        }

        // TODO @sebastianlay: Format event on calendar page as desired.
        return <<<XML
      <article>
        <h2><a href="#" $featherlight_view_data>$pretty->title</a></h2>
        <section class="meta">
          $edit_link_popup $edit_link $pretty->pretty_time, $pretty->location
        </section>
      </article>
XML;
    }
}
