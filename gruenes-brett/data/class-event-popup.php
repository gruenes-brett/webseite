<?php
/**
 * Defines the behavior and appearance of the event popup.
 *
 * @package GruenesBrett
 */

/**
 * Event popup that is called via AJAX.
 */
class Event_Popup extends Comcal_Featherlight_Event_Popup {
    protected static function render( Comcal_Event $event ) : void {
        $pretty = new Pretty_Event( $event );

        $stylesheet_directory = esc_url( get_stylesheet_directory_uri() );

        // TODO @sebastianlay: Format event popup as desired.
        echo <<<XML
    <main class="detail">
      <section class="note">
        <section class="image" style="background-image: url($stylesheet_directory/img/placeholder.png);"></section>
        <h2><a href="$pretty->url" target="_blank" rel="noreferrer noopener">$pretty->title</a></h2>
        <section class="meta">
          $pretty->prettyDate, $pretty->prettyTime, $pretty->location
        </section>
        <section class="share">
          <div class="group">
            <label for="permalink">Link zur Veranstaltung</label>
            <div class="formgroup">
              <input type="text" id="permalink" value="$pretty->url" readonly>
              <button><img src="$stylesheet_directory/img/icons/clipboard-fill.svg" alt="Kopieren"></button>
            </div>
          </div>
          <div class="group">
            <label>Veranstaltung teilen</label>
            <div class="formgroup">
              <button><img src="$stylesheet_directory/img/icons/facebook-fill.svg" alt="Facebook"></button>
              <button><img src="$stylesheet_directory/img/icons/twitter-fill.svg" alt="Twitter"></button>
              <button><img src="$stylesheet_directory/img/icons/telegram-fill.svg" alt="Telegram"></button>
              <button><img src="$stylesheet_directory/img/icons/calendar-event-fill.svg" alt="Kalender"></button>
            </div>
          </div>
        </section>
      </section>
      <article>
        <section class="description">
          $pretty->description
        </section>
      </article>
    </main>
XML;
    }
}

Event_Popup::verify_popup_initialized();
