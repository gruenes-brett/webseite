<?php
/**
 * Template Name: Kategorien bearbeiten
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'addevent' ); ?>
<main class="editcategories">
  <section class="note">
    <?php the_content(); ?>
  </section>

  <?php
    if ( Comcal_User_Capabilities::edit_categories() ) {
        $categories      = Category_Provider::get_all();
        $categories_form = new Comcal_Edit_Categories_Form( $categories );
        echo $categories_form->get_form_html();
    }

    if ( Comcal_User_Capabilities::administer_events() ) {

        ?>
    <section class="adminview">
        <?php
        echo Event_Admin_View_Builder::get_instance()->get_html();
        ?>
    </section>
        <?php
    }
    ?>
</main>
<?php get_footer(); ?>
