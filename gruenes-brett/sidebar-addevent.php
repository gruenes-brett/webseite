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
    <div class="item">
      <a href="">Facebook-Veranstaltung importieren</a>
    </div>

    <!-- TODO: make these links dynamic -->

    <?php if ( ! is_user_logged_in() ) : ?>
    <div class="item">
      <a href="/wp-admin">Anmelden für sofortige Freischaltung</a>
    </div>
    <div class="item">
      <a href="/wer-wir-sind/mitmachen">Account beantragen</a>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::edit_categories() ) : ?>
    <div class="item">
      <a href="/kategorien-bearbeiten">Kategorien bearbeiten</a>
    </div>
    <?php endif; ?>
  </nav>
</aside>
