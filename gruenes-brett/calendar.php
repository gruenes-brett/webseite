<?php
/**
 * Template Name: Kalender
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'calendar' ); ?>

<main class="calendar">

<?php

if ( verify_community_calendar_loaded( true ) ) {
    echo Calendar_Table_Builder::get_instance()->get_html();
    echo_floating_buttons();
}
?>

</main>
<?php get_footer(); ?>
