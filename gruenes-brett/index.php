<?php
/**
 * Default index file
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
?>
</main>
<?php get_footer(); ?>
