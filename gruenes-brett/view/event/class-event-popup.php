<?php
/**
 * Defines the behavior and appearance of the event popup.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Event popup that is called via AJAX.
 */
class Event_Popup extends Comcal_Featherlight_Event_Popup {
    protected static function render( Comcal_Event $event ) : void {
        $detail_view = new Event_Detail_View( $event );
        echo $detail_view->get_main_html();
    }
}

Event_Popup::verify_popup_initialized();
