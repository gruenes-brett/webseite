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

    protected static function render( Comcal_Event $event ) : void {
        $form      = new Edit_Event_Form( '', $event );
        $form_html = $form->get_form_html();

        echo <<<XML
        <main class="addevent" data-controller="form">
            <section class="note">
            </section>

            $form_html
        </main>
        XML;
    }

    /**
     * Render an edit or add event form.
     *
     * @param Comcal_Event $event If set, an 'edit' form will be created. Leaving this empty,
     *                            an 'add' form is shown.
     */
}

Edit_Event_Popup::verify_popup_initialized();
