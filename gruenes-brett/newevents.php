<?php
/**
 * Template Name: Neue Veranstaltungen
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'administration' ); ?>
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
</main>
<?php get_footer(); ?>