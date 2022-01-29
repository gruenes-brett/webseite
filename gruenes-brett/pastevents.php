<?php
/**
 * Template Name: Vergangene Veranstaltungen
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'administration' ); ?>
<main class="calendar">
<h2>Vergangene Veranstaltungen</h2>
<?php
if ( verify_community_calendar_loaded( true ) && Comcal_User_Capabilities::administer_events() ) {
    echo Past_Calendar_Table_Builder::get_instance()->get_html();
    echo_floating_buttons();
}
?>
</main>
<?php get_footer(); ?>