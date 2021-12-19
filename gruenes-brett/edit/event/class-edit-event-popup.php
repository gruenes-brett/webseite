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
 * Edit event popup that is called via AJAX.
 *
 * Use render_function() to directly create the form anywhere on the page.
 */
class Edit_Event_Popup extends Comcal_Featherlight_Event_Popup {

    /**
     * Produces HTML of the links that can be used to edit and copy the event.
     * Links are only created if the current user has editing privileges for the given event.
     *
     * @param Comcal_Event $event Event for which to create the links.
     * @param string       $prefix Optional string to prefix the links.
     */
    public static function create_edit_links( Comcal_Event $event, string $prefix = '' ) {
        $edit_link = '';
        if ( $event->current_user_can_edit() ) {
            $featherlight_edit_data = self::get_featherlight_attribute( $event );
            $edit_link              = "<a href='#' $featherlight_edit_data>bearbeiten</a>";

            $featherlight_edit_copy_data = self::get_featherlight_attribute( $event, true );
            $edit_link                   = "$prefix$edit_link <a href='#' $featherlight_edit_copy_data>kopieren</a>";
        }
        return $edit_link;
    }

    /**
     * Render an edit or add event form.
     *
     * @param Comcal_Event $event If set, an 'edit' form will be created. Leaving this empty,
     *                            an 'add' form is shown.
     */
    protected static function render( Comcal_Event $event ) : void {
        $form      = new Edit_Event_Form( '', $event );
        $form_html = $form->get_form_html();

        echo <<<XML
        <main class="addevent" data-controller="form">
            $form_html
        </main>
        XML;
    }

}

Edit_Event_Popup::verify_popup_initialized();
