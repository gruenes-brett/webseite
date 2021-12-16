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
        $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $event );

        $date = $pretty->formatted_date;
        $time = $pretty->formatted_time;

        if ( '' !== $time ) {
            $time = ', ' . $time;
        }

        $organizer = $pretty->organizer;
        if ( '' !== $organizer ) {
            $organizer = '<br>' . $organizer;
        }

        $edit_link = $this->get_edit_link( $event );
        if ( '' !== $edit_link ) {
            $edit_link = "<a href='#' $featherlight_edit_data>bearbeiten</a>";
        }
        if ( '' !== $date && '' !== $edit_link ) {
            $edit_link = ', ' . $edit_link;
        }

        $private_class  = $event->get_field( 'public' ) ? '' : 'private';
        $obsolete_class = $event->is_obsolete() ? 'obsolete' : '';

        $overlay = $this->get_overlay( $event );

        $title = wp_trim_words( $pretty->title, 10, '...' );

        $description = wp_trim_words( $pretty->description, 60, '...' );

        return <<<XML
        <article class="$private_class $obsolete_class">
            $overlay
            <section class="image" style="background-image: url($image_url);">
                <a href="#" $featherlight_view_data></a>
            </section>
            <h2><a href="#" $featherlight_view_data>$title</a></h2>
            <section class="meta">
            $date$time$organizer$edit_link
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
