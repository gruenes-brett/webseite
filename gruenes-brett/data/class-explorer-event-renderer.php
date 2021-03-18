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
        $title       = $event->get_field( 'title' );
        $time        = $event->get_start_date_time()->get_humanized_time();
        $date        = $event->get_start_date_time()->get_humanized_date();
        $description = $event->get_field( 'description' );
        $url         = $event->get_field( 'url' );
        $image_url   = esc_url( $event->get_field( 'imageUrl' ) );
        if ( ! $image_url ) {
            $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        }

        $edit_link  = $this->get_edit_link( $event );

        $featherlight_data = Event_Popup::get_featherlight_attribute( $event );

        return <<<XML
        <article>
            <section class="image" style="background-image: url($image_url);">
                <a href="#" $featherlight_data></a>
            </section>
            <h2><a href="#" $featherlight_data>$title</a></h2>
            <section class="meta">
            $edit_link $date, $time
            </section>
            <section class="description">
            <p>$description</p>
            <p class="details"><a href="$url">mehr Informationen</a></p>
            </section>
        </article>
XML;
    }
}

Event_Popup::verify_popup_initialized();
