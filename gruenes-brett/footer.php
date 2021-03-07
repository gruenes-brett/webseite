<?php
/**
 * Common footer
 *
 * @package GruenesBrett
 */

?>
  </div>
  <?php wp_enqueue_script( 'jquery' ); ?>
  <?php wp_enqueue_script( 'featherlight', esc_url( get_stylesheet_directory_uri() ) . '/js/featherlight.min.js', '', '1.0', true ); ?>
  <?php wp_footer(); ?>
</body>
</html>
