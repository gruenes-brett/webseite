<?php
/**
 * Defines the edit/add event form.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

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
        'inputName'           => 'submitterName',
        'inputEmail'          => 'submitterEmail',
        'inputTitle'          => 'title',
        'inputStartDate'      => 'date',
        'inputStartTime'      => 'time',
        'inputEndDate'        => 'dateEnd',
        'inputEndTime'        => 'timeEnd',
        'inputPlaceName'      => 'location',
        'inputPlaceAddress'   => 'address',
        'textareaDescription' => 'description',
        'inputOrganizer'      => 'organizer',
        'inputUrl'            => 'url',
        'inputImageUrl'       => 'imageUrl',
        'inputPublic'         => 'public',
        'inputDelete'         => 'delete',
        'inputJoinDaily'      => 'joinDaily',
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
        $event_id     = $this->event->get_entry_id();
        $event_exists = $this->event->exists();

        $placeholder = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );

        $organizer   = $this->event->get_field( 'organizer' );
        $location    = $this->event->get_field( 'location' );
        $address     = $this->event->get_field( 'address' );
        $title       = $this->event->get_field( 'title' );
        $date        = $this->event->get_field( 'date', gmdate( 'Y-m-d' ) );
        $time        = $this->event->get_field( 'time', '12:00' );
        $date_end    = $this->event->get_field( 'dateEnd', '' );
        $time_end    = $this->event->get_field( 'timeEnd', '' );
        $url         = $this->event->get_field( 'url' );
        $description = $this->event->get_field( 'description' );
        $image_url   = $this->event->get_field( 'imageUrl', $placeholder );

        $join_daily_checked = $this->event->get_field( 'joinDaily' ) ? 'checked' : 'unchecked';

        $category_selector     = $this->get_category_selector();
        $submitter_form_fields = $this->get_submitter_form_fields();
        $facebook_import       = $this->get_import_facebook_event_control();

        $more_fields  = $this->get_privacy_consent_checkbox();
        $more_fields .= $this->get_public_checkbox();
        $more_fields .= $event_exists ? $this->get_delete_checkbox() : '';

        $submit_button_text = 'Veranstaltung eintragen';
        if ( $event_exists ) {
            $submit_button_text = 'Veranstaltung aktualisieren';
        }

        $spacer = $this->get_spacer();

        return <<<XML

            <table data-target="form.table">
              $submitter_form_fields
              $facebook_import
              <tr>
                <td><label for="inputTitle">Veranstaltungsname</label></td>
                <td><input type="text" id="inputTitle" name="inputTitle" placeholder="Reinigung der Elbwiesen" maxlength="100" value="$title" data-target="form.title" required></td>
              </tr>
              <tr>
                <td><label for="inputOrganizer">Veranstalter</label></td>
                <td><input type="text" id="inputOrganizer" name="inputOrganizer" placeholder="Organisation XY" maxlength="100" value="$organizer"></td>
              </tr>
              <tr>
                <td><label for="inputUrl">Veranstaltungslink</label></td>
                <td><input type="text" id="inputUrl" name="inputUrl" placeholder="https://..." maxlength="100" data-target="form.url" value="$url"></td>
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
                      <input type="date" name="inputStartDate" id="inputStartDate" data-target="form.startDate" value="$date" required>
                    </div>
                    <div class="row">
                      <label for="inputEndDate">zum</label>
                      <input type="date" name="inputEndDate" id="inputEndDate" data-target="form.endDate" value="$date_end">
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
                      <input type="checkbox" name="inputJoinDaily" id="inputJoinDaily" $join_daily_checked>
                      <label for="inputJoinDaily">Bei mehrtägigen Veranstaltungen: Teilnahme an jedem Tag möglich</label>
                    </div>
                  </div>
                </td>
              </tr>
              $spacer
              <tr>
                <td><label for="inputPlaceName">Veranstaltungsort</label></td>
                <td><input type="text" name="inputPlaceName" id="inputPlaceName" placeholder="Am Blauen Wunder" maxlength="50" value="$location"></td>
              </tr>
              <tr>
                <td><label for="inputPlaceAddress">Adresse</label></td>
                <td><input type="text" name="inputPlaceAddress" id="inputPlaceAddress" placeholder="Musterstr. 42" maxlength="100" data-target="form.placeAddress" value="$address"></td>
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
              $spacer
              <tr>
                <td><label for="textareaDescription">Beschreibung</label></td>
                <td><textarea name="textareaDescription" id="textareaDescription" rows="7" data-target="form.description">$description</textarea></td>
              </tr>

              $category_selector

              <tr>
                <td><label>Veranstaltungsbild</label></td>
                <td>
                  <div class="formgroup">
                    <input type="file" name="inputImage" id="inputImage" data-target="form.image" data-action="form#uploadImage">
                  </div>
                  <div class="formgroup">&nbsp;</div>
                  <div class="formgroup">
                    <div class="image" style="background-image: url('$image_url')" data-target="form.imagePreview"></div>
                    <input type="hidden" value="$image_url" name="inputImageUrl" id="inputImageUrl" data-target="form.imageUrl" />
                  </div>
                  <div class="formgroup">&nbsp;</div>
                  <div class="formgroup">
                    Das Bild wird im Format 16:9 angezeigt werden. Die empfohlene Auflösung ist 960x540 Pixel. 
                  </div>
                </td>
              </tr>
              $spacer
              $more_fields
              $spacer
              <tr>
                <td></td>
                <td><button type="submit">$submit_button_text</button></td>
              </tr>
            </table>
XML;
    }

    protected function get_privacy_consent_checkbox() {
        $checked = Comcal_User_Capabilities::is_logged_in() ? 'checked' : '';
        return <<<XML
              <tr>
                <td><label>Datenschutz</label></td>
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

    protected function get_public_checkbox() {
        if ( ! $this->event->current_user_can_edit() ) {
            return '';
        }
        $public  = $this->event->get_field( 'public', 1 );
        $checked = $public ? 'checked' : '';
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

    protected function get_delete_checkbox() {
        if ( ! $this->event->current_user_can_edit() ) {
            return '';
        }
        return <<<XML
              <tr>
                <td></td>
                <td>
                  <div class="formgroup highlight">
                    <div class="row">
                      <input type="checkbox" name="inputDelete" id="inputDelete">
                      <label for="inputDelete">Event löschen</label>
                    </div>
                  </div>
                </td>
              </tr>
XML;
    }

    protected function get_spacer() {
        return <<<XML
              <tr>
                <td colspan="2">&nbsp;</td>
              </tr>
XML;
    }

    protected function get_submitter_form_fields() : string {
        $event_exists = $this->event->exists();

        $name  = '';
        $email = '';
        if ( $event_exists ) {
            $name  = $this->event->get_field( 'submitterName' );
            $email = $this->event->get_field( 'submitterEmail' );
        } else {
            $user = wp_get_current_user();
            if ( null !== $user ) {
                $name  = $user->user_login;
                $email = $user->user_email;
            }
        }

        $spacer = $this->get_spacer();

        return <<<XML
              <tr>
                <td><label for="inputName">Dein Name</label></td>
                <td><input type="text" name="inputName" id="inputName" placeholder="Max Mustermann" value="$name" required></td>
              </tr>
              <tr>
                <td><label for="inputEmail">E-Mail-Adresse</label></td>
                <td><input type="email" name="inputEmail" id="inputEmail" placeholder="max@mustermann.de" value="$email" required></td>
              </tr>
              $spacer
XML;
    }

    protected function get_import_facebook_event_control() : string {
        return <<<XML
        <tr>
          <td></td>
          <td>
            <a href="#" id="importFacebookEvent" data-action="form#importFacebookEvent">
              Facebook-Veranstaltung importieren
            </a>
          </td>
        </tr>
XML;
    }

    protected function get_category_selector() : string {
        $all_categories     = Category_Provider::get_all();
        $main_cat_options   = '';
        $other_cats_options = '';
        foreach ( $all_categories as $category ) {
            $cat_id        = $category->get_entry_id();
            $name          = $category->get_field( 'name' );
            $selected_main = $this->event->has_category( $category, true ) ? 'selected' : '';
            $selected      = $this->event->has_category( $category, false ) ? 'selected' : '';

            $main_cat_options   .= "<option value='$cat_id' $selected_main>$name</option>\n";
            $other_cats_options .= "<option value='$cat_id' $selected>$name</option>\n";
        }
        return <<<XML
              <tr>
                <td><label for="selectMainCategory">Primäre Kategorie</label></td>
                <td>
                  <select name="selectMainCategory" id="selectMainCategory">
                    $main_cat_options
                  </select>
                </td>
              </tr>
              <tr>
                <td><label for="selectCategories">Weitere Kategorien</label></td>
                <td>
                  <select multiple name="selectCategories[]" id="selectCategories" size="7">
                    $other_cats_options
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
        $primary_category = isset( $post_data['selectMainCategory'] ) ? array( $post_data['selectMainCategory'] ) : array();
        return array_merge( $primary_category, $post_data['selectCategories'] ?? array() );
    }
}

Edit_Event_Form::register_form();
