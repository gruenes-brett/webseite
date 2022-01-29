<?php
/**
 * Template Name: Feeds
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar(); ?>
<main class="feeds">
<?php
    the_title( '<h2>', '</h2>' );
    the_content();

    $base_feed_url = esc_url( get_home_url() ) . '/feed';
    $base_ical_url = esc_url( get_home_url() ) . '/ical/all';
?>
<h3>RSS-Feeds</h3>
<table>
    <tr>
        <td>Alle Kategorien</td>
        <td><a href="<?php echo $base_feed_url; ?>">RSS-Feed</a></td>
        <td><a href="<?php echo $base_ical_url; ?>">iCal-Kalender</a></td>
    </tr>
    <tr><td></td><td></td><td></td></tr>
<?php
$categories = Category_Provider::get_category_names();
foreach ( $categories as $category ) {
    echo '<tr>'
        . "<td>$category</td>"
        . "<td><a href=\"$base_feed_url?category=$category\">RSS-Feed</a></td>"
        . "<td><a href=\"$base_ical_url?category=$category\">iCal-Kalender</a></td>"
        . '</tr>';
}
?>
</table>
</main>
<?php get_footer(); ?>
