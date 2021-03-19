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

        // TODO @sebastianlay: Format event popup as desired.
        echo <<<XML
            <h4 id="title">$pretty->title</h4>
            <p>
                <span id="weekday">$pretty->weekday</span>,
                <span id="prettyDate">$pretty->prettyDate</span> um <span id="prettyTime">$pretty->prettyTime</span></p>
            <p>Ort: <span id="location">$pretty->location</span></p>
            <p>Veranstalter: <span id="organizer">$pretty->organizer</span></p>
            <p><a href="$pretty->url" target="_blank">Veranstaltungslink</a></p>
            <b>Beschreibung:</b>
            <p id="description">$pretty->description</p>
            <p id="categories" class="comcal-categories"></p>
XML;
    }
}
