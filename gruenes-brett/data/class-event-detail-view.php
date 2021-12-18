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

    public function get_aside_html() {
        return <<<XML
            <a>{$this->pretty->title}</a>
XML;

    }

    public function get_main_html() {
        $pretty = $this->pretty;

        $stylesheet_directory = esc_url( get_stylesheet_directory_uri() );

        $image_url = $pretty->image_url;
        if ( ! $image_url ) {
            $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
        }

        $date = $pretty->formatted_date;
        $time = $pretty->formatted_time;

        if ( '' !== $time ) {
            $time = ', ' . $time;
        }

        $location = $pretty->location;
        if ( '' !== $location ) {
            $location = '<br>' . $location;
        }

        $address = $pretty->address;
        if ( '' !== $address ) {
            $address = '<br>' . $address;
        }

        $organizer = $pretty->organizer;
        if ( '' !== $organizer ) {
            $organizer = '<br>' . $organizer;
        }

        $edit_link = '';
        if ( $this->event->current_user_can_edit() ) {
            $featherlight_edit_data = Edit_Event_Popup::get_featherlight_attribute( $this->event );
            $edit_link              = "<br><a href='#' $featherlight_edit_data>bearbeiten</a>";
        }

        $description = make_clickable( $pretty->description );

        $event_link = '';
        if ( $pretty->url ) {
            $event_link = "<br><a href='$pretty->url' class='more'>mehr Informationen "
                          . "<img src='$stylesheet_directory/img/icons/arrow-right-line.svg' alt='Pfeil'></a><br>";
        }

        $permalink = esc_url( get_home_url() . '/veranstaltung/' . $pretty->event_id );

        $dtstamp = $this->event->get_created_date()->format( 'Ymd\THis' );
        $dtstart = $this->event->get_start_date_time( 0 )->format( 'Ymd\THis' );
        $dtend   = $this->event->get_end_date_time()->format( 'Ymd\THis' );

        $ics_start = rawurlencode(
            'BEGIN:VCALENDAR' . PHP_EOL
            . 'VERSION:2.0' . PHP_EOL
            . 'PRODID:-//gruenesbrett//NONSGML v1.0//EN' . PHP_EOL
            . 'BEGIN:VEVENT' . PHP_EOL
            . "UID:$pretty->event_id@gruenesbrett" . PHP_EOL
            . "DTSTAMP:$dtstamp" . PHP_EOL
            . "DTSTART:$dtstart" . PHP_EOL
            . "DTEND:$dtend" . PHP_EOL
            . "SUMMARY:$pretty->title" . PHP_EOL
        );
        $ics_desc  = 'DESCRIPTION: ' . str_replace( '<br />', '\\n', $pretty->description );
        $ics_end   = rawurlencode( PHP_EOL . 'END:VEVENT' . PHP_EOL . 'END:VCALENDAR' );
        $ics       = $ics_start . $ics_desc . $ics_end;

        return <<<XML
    <main class="detail">
      <section class="note">
        <section class="image" style="background-image: url('$image_url');"></section>
        <h2><a href="$permalink">$pretty->title</a></h2>
        <section class="meta">
          $date$time$location$address$organizer$edit_link
        </section>
        <section class="share" data-controller="share">
          <div class="group">
            <label>Veranstaltung teilen</label>
            <div class="formgroup">
              <button data-target="share.facebook" data-action="share#onFacebook"><img src="$stylesheet_directory/img/icons/facebook-fill.svg" alt="Facebook"></button>
              <button data-target="share.twitter" data-action="share#onTwitter"><img src="$stylesheet_directory/img/icons/twitter-fill.svg" alt="Twitter"></button>
              <button data-target="share.telegram" data-action="share#onTelegram"><img src="$stylesheet_directory/img/icons/telegram-fill.svg" alt="Telegram"></button>
              <a href="data:text/calendar;charset=utf8,$ics" download="cal.ics"><img src="$stylesheet_directory/img/icons/calendar-event-fill.svg" alt="Kalender"></a>
            </div>
          </div>
          <div class="group">
            <div class="formgroup">
              <input type="text" id="permalink" data-target="share.permalink" value="$permalink" readonly>
              <button data-target="share.clipboard" data-action="share#copyPermalink">
                <img src="$stylesheet_directory/img/icons/clipboard-fill.svg" alt="Kopieren" data-target="share.clipboardIcon">
              </button>
            </div>
          </div>
        </section>
      </section>
      <article>
        <section class="description">
          $description
          $event_link
        </section>
      </article>
    </main>
XML;

    }
}
