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
    public function render( Comcal_Event $event, int $day ) : string {
        if ( 0 !== $day ) {
            // Render multi-day events only once.
            return '';
        }
        $pretty = new Pretty_Event( $event );

        $image_url = $pretty->image_url;
        if ( ! $image_url ) {
            $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        }

        $featherlight_view_data = Event_Popup::get_featherlight_attribute( $event );

        $date = $pretty->formatted_date;
        $time = $pretty->formatted_time;
        $meta = '';
        if ( $event->is_cancelled() ) {
            $meta = $pretty->cancelled_html . '<br>';
        }
        if ( ! $pretty->is_multiday && '' !== $time ) {
            // Hide time for multiday events.
            $meta .= "$date, $time";
        } else {
            $meta .= $date;
        }

        $meta .= Edit_Event_Popup::create_edit_links( $event, ', ' );

        $organizer = $pretty->organizer;
        if ( '' !== $organizer ) {
            $meta .= '<br>' . $organizer;
        }

        $private_class = $event->get_field( 'public' ) ? '' : 'private';

        $overlay = $this->get_overlay( $event );

        $title = wp_trim_words( $pretty->name, 10, '...' );

        $description = wp_trim_words( $pretty->description, 60, '...' );

        return <<<XML
        <article class="$private_class">
            $overlay
            <section class="image" style="background-image: url($image_url);">
                <a href="#" $featherlight_view_data></a>
            </section>
            <h2><a href="#" $featherlight_view_data>$title</a></h2>
            <section class="meta">
            $meta
            </section>
            <section class="description">
            <p>$description</p>
            <p class="details"><a href="#" $featherlight_view_data>mehr Informationen</a></p>
            </section>
        </article>
XML;
    }

    protected function get_overlay( Comcal_Event $event ) : string {
        $background_color = 'var(--green)';
        $category         = $event->get_primary_category();
        if ( null !== $category ) {
            $background_color = $category->get_background_color();
        }
        return <<<XML
            <div class="event-overlay" style="background-color: $background_color;"></div>
XML;
    }
}

Event_Popup::verify_popup_initialized();
