<?php
/**
 * Template Name: Kategorien bearbeiten
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'administration' ); ?>
<main class="adminview">
  <section class="categories" data-controller="categories">
  <?php
    if ( Comcal_User_Capabilities::edit_categories() ) {
        $categories      = Category_Provider::get_all();
        $categories_form = new Edit_Categories_Form( $categories );
        echo $categories_form->get_form_html();
    }

    ?>
  </section>
</main>
<?php get_footer(); ?>
