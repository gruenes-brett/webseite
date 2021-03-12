<?php
/**
 * Class definition for Explorer_Event_Renderer.
 *
 * @package GruenesBrett
 */

/**
 * Defines how an event is rendered in the explorer view.
 */
class Explorer_Event_Renderer extends comcal_DefaultEventRenderer {
    public function render( comcal_Event $event ) : string {
        $title       = $event->getField( 'title' );
        $time        = $event->getDateTime()->getHumanizedTime();
        $date        = $event->getDateTime()->getHumanizedDate();
        $description = $event->getField( 'description' );
        $url         = $event->getField( 'url' );
        $edit_link   = $this->get_edit_link( $event );
        $image_url   = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        return <<<XML
        <article>
            <section class="image" style="background-image: url($image_url);">
                <a href=""></a>
            </section>
            <h2><a href="">$title</a></h2>
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
