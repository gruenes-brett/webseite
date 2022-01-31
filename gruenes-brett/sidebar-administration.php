<?php
/**
 * Navigation bar for the administrative pages
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded( true ) ) {
    return;
}

?>
<aside>
  <nav>
    <!-- TODO: make these links dynamic -->

    <?php if ( ! is_user_logged_in() ) : ?>
    <div class="item">
      <a href="/wp-login.php?redirect_to=/veranstaltung-eintragen">Anmelden für sofortige Freischaltung</a>
    </div>
    <div class="item">
      <a href="/wer-wir-sind/">Account beantragen</a>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::has_edit_privileges() ) : ?>
    <div class="item">
      <a href="/veranstaltung-eintragen/neue-veranstaltungen/">Neueste Veranstaltungen</a>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::administer_events() ) : ?>
    <div class="item">
      <a href="/veranstaltung-eintragen/vergangene-veranstaltungen/">Vergangene Veranstaltungen</a>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::edit_categories() ) : ?>
    <div class="item">
      <a href="/veranstaltung-eintragen/kategorien-bearbeiten/">Kategorien bearbeiten</a>
    </div>
    <?php endif; ?>
  </nav>
</aside>
