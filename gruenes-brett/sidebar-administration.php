<?php
/**
 * Navigation bar for the administrative pages
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded( true ) ) {
    return;
}

$register_url = '/wer-wir-sind/';
if ( '1' === get_option( 'users_can_register' ) ) {
    $register_url = '/wp-login.php?action=register';
}

function echo_link( $target, $text ) {
    echo '<a href="' . home_url( $target ) . "\">$text</a>";
}

?>
<aside>
  <nav>
    <!-- TODO: make these links dynamic -->

    <?php if ( ! is_user_logged_in() ) : ?>
    <div class="item">
        <?php echo_link( '/wp-login.php?redirect_to=veranstaltung-eintragen', 'Anmelden für sofortige Freischaltung' ); ?>
    </div>
    <div class="item">
        <?php echo_link( $register_url, 'Account beantragen' ); ?>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::has_edit_privileges() ) : ?>
    <div class="item">
        <?php echo_link( '/veranstaltung-eintragen/neue-veranstaltungen/', 'Neueste Veranstaltungen' ); ?>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::administer_events() ) : ?>
    <div class="item">
        <?php echo_link( '/veranstaltung-eintragen/vergangene-veranstaltungen/', 'Vergangene Veranstaltungen' ); ?>
    </div>
    <?php endif; ?>

    <?php if ( Comcal_User_Capabilities::edit_categories() ) : ?>
    <div class="item">
        <?php echo_link( '/veranstaltung-eintragen/kategorien-bearbeiten/', 'Kategorien bearbeiten' ); ?>
    </div>
    <?php endif; ?>
  </nav>
</aside>
