<?php
/**
 * Defines the behavior and appearance of the event popup.
 *
 * @package GruenesBrett
 */

/**
 * Event popup.
 */
class Event_Popup extends Comcal_Featherlight_Event_Popup {
    protected static function render( Comcal_Event $event ) : void {
        // TODO schöner machen
        $title = $event->get_field( 'title' );
        echo <<<XML
            <h4 id="title">$title</h4>
            <p><span id="weekday"></span>, <span id="prettyDate"></span> um <span id="prettyTime"></span></p>
            <p>Ort: <span id="location"></span></p>
            <p>Veranstalter: <span id="organizer"></span></p>
            <p><a id="url" target="_blank"></a></p>
            <b>Beschreibung:</b>
            <p id="description"></b>
            <p id="categories" class="comcal-categories"></p>
XML;
    }
}
