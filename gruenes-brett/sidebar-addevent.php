<?php
/**
 * Navigation bar for the addevent page
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded( true ) ) {
    return;
}

?>
<aside>
  <nav>
    <!--

      !!! This has been moved into the edit-event-form to make it work with Stimulus

       <div class="item">
      <a href="#" id="importFacebookEvent" data-target="form.placeAddress" data-action="form#importFacebookEvent">Facebook-Veranstaltung importieren</a>
    </div> -->

    <!-- TODO: make these links dynamic -->

    <?php if ( ! is_user_logged_in() ) : ?>
    <div class="item">
      <a href="/wp-admin">Anmelden für sofortige Freischaltung</a>
    </div>
    <div class="item">
      <a href="/wer-wir-sind/mitmachen">Account beantragen</a>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::administer_events() ) : ?>
    <div class="item">
      <a href="/kategorien-bearbeiten">Administration</a>
    </div>
    <?php endif; ?>
  </nav>
</aside>
