<?php
/**
 * Template Name: Feeds
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar(); ?>
<main>
<?php
    the_title( '<h2>', '</h2>' );
    the_content();

    $base_url = esc_url( get_home_url() ) . '/feed';
?>
<h3>RSS-Feeds</h3>
<p><a href="<?php echo $base_url; ?>">Alle Kategorien</a></p>
<?php

$categories = Category_Provider::get_category_names();
foreach ( $categories as $category ) {
    echo "<p><a href=\"$base_url?category=$category\">$category</a></p>";
}
?>
</main>
<?php get_footer(); ?>
