<?php
/**
 * Template Name: Kategorien bearbeiten
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<main class="adminview">

  <section class="eventhistory">
    <?php

    if ( Comcal_User_Capabilities::administer_events() ) {

        echo Event_Admin_View_Builder::get_instance()->get_html();
    }
    ?>
  </section>

  <section class="categories">
  <?php
    if ( Comcal_User_Capabilities::edit_categories() ) {
        $categories      = Category_Provider::get_all();
        $categories_form = new Comcal_Edit_Categories_Form( $categories );
        echo $categories_form->get_form_html();
    }

    ?>
  </section>

</main>
<?php get_footer(); ?>
