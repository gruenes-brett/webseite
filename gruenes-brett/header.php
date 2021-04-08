<?php
/**
 * Common header
 *
 * @package GruenesBrett
 */

?>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Grünes Brett - Erkunden</title>
  <?php wp_head(); ?>
  <link rel="icon" href="<?php echo esc_url( get_stylesheet_directory_uri() ); ?>/img/favicon.png">
  <link rel="shortcut icon" href="<?php echo esc_url( get_stylesheet_directory_uri() ); ?>/img/favicon.png">
</head>
<body>
  <div class="wrapper">
    <header>
      <h1 class="logo">
        <a href="<?php echo esc_url( get_home_url() ); ?>">
          <img src="<?php echo esc_url( get_stylesheet_directory_uri() ); ?>/img/logo.svg" alt="Grünes Brett">
          <span>Grünes Brett</span>
        </a>
      </h1>
<?php

// TODO: move explore and calendar page into the same div!
$output = wp_page_menu(
    array(
        'echo'      => 0,
        'depth'     => 1,
        'container' => 'nav',
        'before'    => '',
        'after'     => '',
    )
);

// TODO: replace with custom page walker in the wp_page_menu call!
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div $1>$2</div>', $output ) );
?>
    </header>
