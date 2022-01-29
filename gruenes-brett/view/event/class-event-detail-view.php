<?php
/**
 * Renders events details.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Helper class for creating event detail HTML.
 */
class Event_Detail_View {
    public function __construct( ?Comcal_Event $event ) {
        if ( null === $event ) {
            $event = new Comcal_Event(
                array(
                    'title'       => 'Invalid Event',
                    'description' => 'Dieses Event konnte nicht gefunden werden.',
                )
            );
        }
        $this->event  = $event;
        $this->pretty = new Pretty_Event( $this->event );
    }

    public function get_main_html() {
        $pretty = $this->pretty;

        $stylesheet_directory = esc_url( get_stylesheet_directory_uri() );

        $image_url = $pretty->image_url;
        if ( ! $image_url ) {
            $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        }

        $edit_link = Edit_Event_Popup::create_edit_links( $this->event, '<br>' );

        $event_link = '';
        if ( $pretty->url ) {
            $event_link = "<br><a href='$pretty->url' class='more'>mehr Informationen "
                          . "<img src='$stylesheet_directory/img/icons/arrow-right-line.svg' alt='Pfeil'></a><br>";
        }

        $private_class = $this->event->get_field( 'public' ) ? '' : 'private';
        $ical_link     = esc_url( get_home_url() ) . '/ical/' . $pretty->event_id;

        return <<<XML
    <main class="detail">
      <section class="note $private_class">
        <section class="image" style="background-image: url('$image_url');"></section>
        <h2><a href="$pretty->permalink">$pretty->name</a></h2>
        <section class="meta">
          $pretty->meta$edit_link
        </section>
        <section class="share" data-controller="share">
          <div class="group">
            <label>Veranstaltung teilen</label>
            <div class="formgroup">
              <button data-target="share.facebook" data-action="share#onFacebook"><img src="$stylesheet_directory/img/icons/facebook-fill.svg" alt="Facebook"></button>
              <button data-target="share.twitter" data-action="share#onTwitter"><img src="$stylesheet_directory/img/icons/twitter-fill.svg" alt="Twitter"></button>
              <button data-target="share.telegram" data-action="share#onTelegram"><img src="$stylesheet_directory/img/icons/telegram-fill.svg" alt="Telegram"></button>
              <a href="$ical_link"><img src="$stylesheet_directory/img/icons/calendar-event-fill.svg" alt="Kalender"></a>
            </div>
          </div>
          <div class="group">
            <div class="formgroup">
              <input type="text" id="permalink" data-target="share.permalink" value="$pretty->permalink" readonly>
              <button data-target="share.clipboard" data-action="share#copyPermalink">
                <img src="$stylesheet_directory/img/icons/clipboard-fill.svg" alt="Kopieren" data-target="share.clipboardIcon">
              </button>
            </div>
          </div>
        </section>
      </section>
      <article>
        <section class="description">
          $pretty->clickable_description
          $event_link
        </section>
      </article>
    </main>
    $pretty->json_schema
XML;

    }
}
