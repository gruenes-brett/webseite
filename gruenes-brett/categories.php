<?php
/**
 * Template Name: Kategorien bearbeiten
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
    if ( Comcal_User_Capabilities::edit_categories() ) {
        $categories      = Category_Provider::get_all();
        $categories_form = new Comcal_Edit_Categories_Form( $categories );
        echo $categories_form->get_form_html();
    }
    ?>
</main>
<?php get_footer(); ?>
