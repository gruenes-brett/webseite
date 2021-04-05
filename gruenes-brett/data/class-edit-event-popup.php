<?php
/**
 * Defines the behavior and appearance of the event popup.
 *
 * @package GruenesBrett
 */

/**
 * Edit event popup that is called via AJAX.
 *
 * Use render_function() to directly create the form anywhere on the page.
 */
class Edit_Event_Popup extends Comcal_Featherlight_Event_Popup {
    protected static function render( Comcal_Event $event ) : void {
        static::render_form( $event );
    }

    /**
     * Render an edit or add event form.
     *
     * @param Comcal_Event $event If set, an 'edit' form will be created. Leaving this empty,
     *                            an 'add' form is shown.
     */
    public static function render_form( Comcal_Event $event = null ) : void {
        if ( null === $event ) {
            $event = new Comcal_Event();
        }

        echo <<<XML
  <form action="" data-controller="form">
    <table>
      <tr>
        <td><label for="inputName">Dein Name</label></td>
        <td><input type="text" id="inputName" placeholder="Max Mustermann" required></td>
      </tr>
      <tr>
        <td><label for="inputEmail">E-Mail-Adresse</label></td>
        <td><input type="email" id="inputEmail" placeholder="max@mustermann.de" required></td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td><label for="inputTitle">Veranstaltungsname</label></td>
        <td><input type="text" id="inputTitle" placeholder="Party im Hinterhof" maxlength="100" required></td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td><label>Datum</label></td>
        <td>
          <div class="formgroup">
            <div class="row">
              <label for="inputStartDate">vom</label>
              <input type="date" id="inputStartDate" required>
            </div>
            <div class="row">
              <label for="inputEndDate">zum</label>
              <input type="date" id="inputEndDate" required>
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
              <input type="time" id="inputStartTime" data-target="form.startTime">
            </div>
            <div class="row">
              <label for="inputEndTime">bis</label>
              <input type="time" id="inputEndTime" data-target="form.endTime">
            </div>
          </div>
        </td>
      </tr>
      <tr>
        <td></td>
        <td>
          <div class="formgroup">
            <div class="row">
              <input type="checkbox" id="inputFullDay" data-target="form.fullDay" data-action="form#setTimeState">
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
        <td><input type="text" id="inputPlaceName" placeholder="Unser Hinterhof" maxlength="50" required></td>
      </tr>
      <tr>
        <td><label for="inputPlaceAddress">Adresse</label></td>
        <td><input type="text" id="inputPlaceAddress" placeholder="Musterstr. 42" maxlength="100" data-target="form.placeAddress"></td>
      </tr>
      <tr>
        <td></td>
        <td>
          <div class="formgroup">
            <div class="row">
              <input type="checkbox" id="inputPhysicalSpace" data-target="form.physicalSpace" data-action="form#setAddressState">
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
        <td><textarea id="textareaDescription" rows="7"></textarea></td>
      </tr>
      <tr>
        <td><label for="selectCategory">Kategorie</label></td>
        <td>
          <select multiple id="selectCategory" size="7">
            <option>Demo</option>
            <option>Diskussion</option>
            <option>Exkursion</option>
            <option>Online</option>
            <option>Pflegeeinsatz</option>
            <option>Sonstiges</option>
            <option>Tauschen</option>
            <option>Treffen</option>
            <option>Vortrag</option>
            <option>Workshop</option>
          </select>
        </td>
      </tr>
      <tr>
        <td><label for="inputImage">Veranstaltungsbild</label></td>
        <td><input type="file" id="inputImage"></td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td></td>
        <td>
          <div class="formgroup">
            <div class="row">
              <input type="checkbox" id="inputPrivacy" required>
              <label for="inputPrivacy">Ich stimme zu, dass meine eingegebenen Daten, wie in der <a href="">Datenschutzerklärung</a> vom Grünen Brett beschrieben, gesammelt und verarbeitet werden.</label>
            </div>
          </div>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td></td>
        <td><button type="submit">Veranstaltung eintragen</button></td>
      </tr>
    </table>
  </form>
XML;
    }
}

Edit_Event_Popup::verify_popup_initialized();
