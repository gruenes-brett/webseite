<?php
/**
 * Template Name: Veranstaltung eintragen
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'administration' ); ?>
<main class="addevent" data-controller="form">
  <section class="note">
    <?php the_content(); ?>
  </section>
  <?php Edit_Event_Form::render_empty_form(); ?>
</main>
<?php get_footer(); ?>
