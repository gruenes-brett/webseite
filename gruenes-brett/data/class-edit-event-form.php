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

    /**
     * Maps between form field name and event field name.
     *
     * @var array
     */
    protected static $form_field_to_model_field = array(
        'inputTitle'     => 'title',
        'inputStartDate' => 'date',
        'inputStartTime' => 'time',
        'inputEndDate'   => 'dateEnd',
        'inputEndTime'   => 'timeEnd',
        'inputPlaceName' => 'location',
        'inputOrganizer' => 'organizer',
        'inputUrl'       => 'url',
        'inputImageUrl'  => 'imageUrl',
        'inputPublic'    => 'public',
    );

    protected function get_form_id(): string {
        return 'edit-event-form';
    }

    protected function get_html_after_form(): string {
        // "hack" to make sure this form can be submitted via AJAX, as
        // defined in forms.js
        $id = $this->get_form_id();
        return <<<XML
          <script>
            jQuery(document).ready(function(){
                register_form_ajax_submission('#$id');
            });
          </script>
XML;

    }

    public function get_form_fields() : string {
        $event_id = $this->event->get_entry_id();

        $organizer   = $this->event->get_field( 'organizer' );
        $location    = $this->event->get_field( 'location' );
        $title       = $this->event->get_field( 'title' );
        $date        = $this->event->get_field( 'date', gmdate( 'Y-m-d' ) );
        $time        = $this->event->get_field( 'time', '12:00:00' );
        $date_end    = $this->event->get_field( 'dateEnd', '' );
        $time_end    = $this->event->get_field( 'timeEnd', '' );
        $url         = $this->event->get_field( 'url' );
        $description = $this->event->get_field( 'description' );
        $image_url   = $this->event->get_field( 'imageUrl' );
        $public      = $this->event->get_field( 'public' );

        $category_selector     = $this->get_category_selector();
        $submitter_form_fields = $this->get_submitter_form_fields();

        $more_fields  = $this->get_privacy_consent_checkbox();
        $more_fields .= $this->get_public_checkbox( $public );

        $submit_button_text = 'Veranstaltung eintragen';
        if ( $this->event->exists() ) {
            $submit_button_text = 'Veranstaltung aktualisieren';
        }

        return <<<XML
            <table>
              $submitter_form_fields
              <tr>
                <td><label for="inputTitle">Veranstaltungsname</label></td>
                <td><input type="text" id="inputTitle" name="inputTitle" placeholder="Party im Hinterhof" maxlength="100" value="$title" required></td>
              </tr>
              <tr>
                <td><label for="inputOrganizer">Veranstalter</label></td>
                <td><input type="text" id="inputOrganizer" name="inputOrganizer" placeholder="Organisation XY" maxlength="100" value="$organizer"></td>
              </tr>
              <tr>
                <td><label for="inputUrl">Veranstaltungslink</label></td>
                <td><input type="text" id="inputUrl" name="inputUrl" placeholder="https://..." maxlength="100" value="$url"></td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>
                    <input name="eventId" id="eventId" value="$event_id" type="hidden">
                </td>
              </tr>
              <tr>
                <td><label>Datum</label></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <label for="inputStartDate">vom</label>
                      <input type="date" name="inputStartDate" id="inputStartDate" value="$date" required>
                    </div>
                    <div class="row">
                      <label for="inputEndDate">zum</label>
                      <input type="date" name="inputEndDate" id="inputEndDate" value="$date_end">
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td><label>Uhrzeit</label></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <label for="inputStartTime">von</label>
                      <input type="time" name="inputStartTime" id="inputStartTime" data-target="form.startTime" value="$time">
                    </div>
                    <div class="row">
                      <label for="inputEndTime">bis</label>
                      <input type="time" name="inputEndTime" id="inputEndTime" data-target="form.endTime" value="$time_end">
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <input type="checkbox" name="inputFullDay" id="inputFullDay" data-target="form.fullDay" data-action="form#setTimeState">
                      <label for="inputFullDay">Es ist eine ganztägige Veranstaltung.</label>
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td><label for="inputPlaceName">Veranstaltungsort</label></td>
                <td><input type="text" name="inputPlaceName" id="inputPlaceName" placeholder="Unser Hinterhof" maxlength="50" value="$location"></td>
              </tr>
              <tr>
                <td><label for="inputPlaceAddress">Adresse</label></td>
                <td><input type="text" name="inputPlaceAddress" id="inputPlaceAddress" placeholder="Musterstr. 42" maxlength="100" data-target="form.placeAddress"></td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <input type="checkbox" name="inputPhysicalSpace" id="inputPhysicalSpace" data-target="form.physicalSpace" data-action="form#setAddressState">
                      <label for="inputPhysicalSpace">Diese Veranstaltung hat keinen physischen Ort.</label>
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td><label for="textareaDescription">Beschreibung</label></td>
                <td><textarea name="textareaDescription" id="textareaDescription" rows="7">$description</textarea></td>
              </tr>

              $category_selector

              <tr>
                <td><label for="inputImage">Veranstaltungsbild</label></td>
                <!-- <td><input type="file" name="inputImage" id="inputImage" ></td> -->
                <td><input type="text" value="$image_url" name="inputImageUrl" id="nputImageUrl" placeholder="https://..."></td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              $more_fields
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td></td>
                <td><button type="submit">$submit_button_text</button></td>
              </tr>
            </table>
XML;
    }

    protected function get_privacy_consent_checkbox() {
        $checked = user_can_administer_events() ? 'checked' : '';
        return <<<XML
              <tr>
                <td></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <input type="checkbox" name="inputPrivacy" id="inputPrivacy" $checked required>
                      <label for="inputPrivacy">Ich stimme zu, dass meine eingegebenen Daten, wie in der <a href="">Datenschutzerklärung</a> vom Grünen Brett beschrieben, gesammelt und verarbeitet werden.</label>
                    </div>
                  </div>
                </td>
              </tr>
XML;
    }

    protected function get_public_checkbox( bool $is_checked ) {
        if ( ! user_can_administer_events() ) {
            return '';
        }
        $checked = $is_checked ? 'checked' : '';
        return <<<XML
              <tr>
                <td></td>
                <td>
                  <div class="formgroup">
                    <div class="row">
                      <input type="checkbox" name="inputPublic" id="inputPublic" $checked>
                      <label for="inputPublic">Event veröffentlichen</label>
                    </div>
                  </div>
                </td>
              </tr>
XML;
    }

    protected function get_submitter_form_fields() : string {
        if ( user_can_administer_events() ) {
            return '';
        }

        return <<<XML
              <tr>
                <td><label for="inputName">Dein Name</label></td>
                <td><input type="text" name="inputName" id="inputName" placeholder="Max Mustermann" required></td>
              </tr>
              <tr>
                <td><label for="inputEmail">E-Mail-Adresse</label></td>
                <td><input type="email" name="inputEmail" id="inputEmail" placeholder="max@mustermann.de" required></td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
XML;
    }

    protected function get_category_selector() : string {
        $all_categories = Category_Provider::get_all();
        $options        = '';
        foreach ( $all_categories as $category ) {
            $cat_id   = $category->get_entry_id();
            $name     = $category->get_field( 'name' );
            $selected = $this->event->has_category( $category ) ? 'selected' : '';
            $options .= "<option value='$cat_id' $selected>$name</option>\n";
        }
        return <<<XML
              <tr>
                <td><label for="selectCategories">Weitere Kategorien</label></td>
                <td>
                  <select multiple name="selectCategories[]" id="selectCategories" size="7">
                    $options
                  </select>
                </td>
              </tr>
XML;
    }

    /**
     * Extracts catgory ids from post data.
     *
     * @param array $post_data Post data.
     */
    protected static function extract_category_ids( $post_data ) {
        return $post_data['selectCategories'] ?? array();
    }
}

Edit_Event_Form::register_form();
