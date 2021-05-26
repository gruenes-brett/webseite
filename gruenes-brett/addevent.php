<?php
/**
 * Template Name: Veranstaltung eintragen
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'addevent' ); ?>
<main class="addevent">
  <section class="note">
    <?php the_content(); ?>
  </section>

  <?php
    if ( Comcal_User_Capabilities::administer_events() ) {
        $categories      = Category_Provider::get_all();
        $categories_form = new Comcal_Edit_Categories_Form( $categories );
        echo $categories_form->get_form_html();
    }

    Edit_Event_Form::render_empty_form();
    ?>
</main>
<?php get_footer(); ?>
