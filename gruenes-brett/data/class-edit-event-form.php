<?php
/**
 * Defines the edit/add event form.
 *
 * @package GruenesBrett
 */

/**
 * Defines the form fields and layout.
 */
class Edit_Event_Form extends Comcal_Edit_Event_Form {
    /**
     * Specifies the action name.
     *
     * @var string
     */
    protected static $action_name = 'submit_event_data';

}

Edit_Event_Form::register_form();
