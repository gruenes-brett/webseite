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

    if ( Comcal_User_Capabilities::has_edit_privileges() ) {
        echo <<<XML
        <h2>Veranstaltungen nach Eintragungsdatum</h2>
XML;
        echo Event_Admin_View_Builder::get_instance()->get_html();
    }
    ?>
  </section>

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
