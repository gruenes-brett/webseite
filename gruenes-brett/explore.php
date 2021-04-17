<?php
/**
 * Template Name: Erkunden
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'explore' ); ?>
<main class="explore">

<?php
if ( verify_community_calendar_loaded( true ) ) {
    echo Event_Explorer_Builder::get_instance()->get_html();
    echo_floating_buttons();
}

?>

</main>
<?php get_footer(); ?>
