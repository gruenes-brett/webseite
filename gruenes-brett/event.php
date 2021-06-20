<?php
/**
 * Template Name: Veranstaltung
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>

<?php
  $event_id    = get_query_var( 'event_id' );
  $event       = Comcal_Event::query_by_entry_id( $event_id );
  $detail_view = new Event_Detail_View( $event );
?>
<aside>
  <nav>
    <div class="item">
      <?php echo $detail_view->get_aside_html(); ?>
    </div>
  <nav>
</aside>

<?php
  echo $detail_view->get_main_html();
?>

<?php get_footer(); ?>
