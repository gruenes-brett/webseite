<?php
/**
 * Template Name: Veranstaltung
 *
 * @package GruenesBrett
 */

?>

<?php

$event_id    = get_query_var( 'event_id' );
$event       = Comcal_Event::query_by_entry_id( $event_id );
$detail_view = new Event_Detail_View( $event );
$image_url   = $detail_view->pretty->image_url;
if ( ! $image_url ) {
    $image_url = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
}

get_header(
    null,
    array(
        'title'       => wp_trim_words( $detail_view->pretty->title, 10, '...' ),
        'description' => wp_trim_words( $detail_view->pretty->description, 60, '...' ),
        'image'       => $image_url,
    )
);
?>

<?php
  echo $detail_view->get_main_html();
?>

<?php get_footer(); ?>
