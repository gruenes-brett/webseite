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
class Explorer_Event_Renderer extends Comcal_Default_Event_Renderer {
    public function render( Comcal_Event $event ) : string {
        $pretty = new Pretty_Event( $event );

        $image_url = $pretty->image_url;
        if ( ! $image_url ) {
            $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        }

        $featherlight_view_data = Event_Popup::get_featherlight_attribute( $event );
        $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $event );

        $edit_link = $this->get_edit_link( $event );
        if ( '' !== $edit_link ) {
            $edit_link = ", <a href='#' $featherlight_edit_data>bearbeiten</a>";
        }

        $private = $event->get_field( 'public' ) ? '' : 'private';

        // TODO @sebastianlay: Format event on explorer page as desired.
        return <<<XML
        <article class="$private">
            <section class="image" style="background-image: url($image_url);">
                <a href="#" $featherlight_view_data></a>
            </section>
            <h2><a href="#" $featherlight_view_data>$pretty->title</a></h2>
            <section class="meta">
            $pretty->pretty_date, $pretty->pretty_time$edit_link
            </section>
            <section class="description">
            <p>$pretty->description</p>
            <p class="details"><a href="#" $featherlight_view_data>mehr Informationen</a></p>
            </section>
        </article>
XML;
    }
}

Event_Popup::verify_popup_initialized();
