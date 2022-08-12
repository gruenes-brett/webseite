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
  <title><?php wp_title( '-', true, 'right' ); ?><?php bloginfo( 'name' ); ?> <?php bloginfo( 'description' ); ?></title>
  <meta property="og:type" content="website">
  <meta property="og:site_name" content="<?php bloginfo( 'name' ); ?>">
  <meta property="og:title" content="<?php echo wp_strip_all_tags( $title ); ?>">
  <meta property="og:description" content="<?php echo wp_strip_all_tags( $description ); ?>">
  <meta property="og:image" content="<?php echo $image; ?>">
  <meta property="og:image:secure_url" content="<?php echo $image; ?>">

  <meta property="twitter:card" content="summary_large_image">
  <meta property="twitter:title" content="<?php echo wp_strip_all_tags( $title ); ?>">
  <meta property="twitter:description" content="<?php echo wp_strip_all_tags( $description ); ?>">
  <meta property="twitter:image" content="<?php echo $image; ?>">

  <?php wp_head(); ?>
  <link rel="icon" href="<?php echo get_image_url( 'favicon.png' ); ?>">
  <link rel="shortcut icon" href="<?php echo get_image_url( 'favicon.png' ); ?>">
  <link rel="alternate" type="application/rss+xml" title="<?php bloginfo( 'name' ); ?> RSS Feed" href="<?php echo esc_url( get_home_url() ); ?>/feed" />
</head>
<body>
  <div class="wrapper">
    <header>
      <h1 class="logo">
        <a href="<?php echo esc_url( get_home_url() ); ?>">
          <img src="<?php echo get_logo_url(); ?>" alt="<?php bloginfo( 'name' ); ?>">
          <span><?php bloginfo( 'description' ); ?></span>
        </a>
      </h1>
      <nav class="menu">
        <div class="row">
<?php

$event_page    = get_page_by_path( 'veranstaltung' )->ID;
$explore_page  = get_page_by_path( 'erkunden' )->ID;
$calendar_page = get_page_by_path( 'kalender' )->ID;

$explore_entry = wp_page_menu(
    array(
        'echo'       => 0,
        'depth'      => 1,
        'container'  => 'false',
        'before'     => '',
        'after'      => '',
        'include'    => $explore_page,
        'items_wrap' => '%3$s',
    )
);

$calendar_entry = wp_page_menu(
    array(
        'echo'       => 0,
        'depth'      => 1,
        'container'  => 'false',
        'before'     => '',
        'after'      => '',
        'include'    => $calendar_page,
        'items_wrap' => '%3$s',
    )
);
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div $1>$2</div>', $explore_entry ) );
echo '<div>/</div>';
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div $1>$2</div>', $calendar_entry ) );
?>
        </div>
<?php
$other_entries = wp_page_menu(
    array(
        'echo'       => 0,
        'depth'      => 1,
        'container'  => 'false',
        'before'     => '',
        'after'      => '',
        'exclude'    => "$explore_page,$calendar_page,$event_page",
        'items_wrap' => '%3$s',
    )
);
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div $1>$2</div>', $other_entries ) );
?>
      </nav>
    </header>
