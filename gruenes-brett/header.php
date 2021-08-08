<?php
/**
 * Common header
 *
 * @package GruenesBrett
 */

$defaults = array(
    'title'       => get_bloginfo( 'name' ),
    'description' => get_bloginfo( 'description' ),
    'image'       => esc_url( get_stylesheet_directory_uri() ) . '/img/preview.png',
);
extract( wp_parse_args( $args, $defaults ) );

?>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Grünes Brett - Erkunden</title>
  <meta property="og:type" content="website">
  <meta property="og:url" content="<?php echo esc_url( get_home_url() ); ?>">
  <meta property="og:title" content="<?php echo html_entity_decode( $title ); ?>">
  <meta property="og:description" content="<?php echo html_entity_decode( $description ); ?>">
  <meta property="og:image" content="<?php echo $image; ?>">
  <meta property="og:image:secure_url" content="<?php echo $image; ?>">

  <meta property="twitter:card" content="summary_large_image">
  <meta property="twitter:url" content="<?php echo esc_url( get_home_url() ); ?>">
  <meta property="twitter:title" content="<?php echo html_entity_decode( $title ); ?>">
  <meta property="twitter:description" content="<?php echo html_entity_decode( $description ); ?>">
  <meta property="twitter:image" content="<?php echo $image; ?>">
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
          <!-- <span>Grünes Brett</span> -->
        </a>
      </h1>
<?php

// TODO: move explore and calendar page into the same div and hide "veranstaltung" properly!
$output = wp_page_menu(
    array(
        'echo'      => 0,
        'depth'     => 1,
        'container' => 'nav',
        'before'    => '',
        'after'     => '',
        'exclude'   => 67,
    )
);

// TODO: replace with custom page walker in the wp_page_menu call!
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div $1>$2</div>', $output ) );
?>
    </header>
