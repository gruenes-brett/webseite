<?php
/**
 * Template Name: Veranstaltung
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>

<?php
  $stylesheet_directory = esc_url( get_stylesheet_directory_uri() );
  $image_url            = esc_url( get_stylesheet_directory_uri() . '/img/placeholder.png' );
  $permalink            = esc_url( get_home_url() . '/veranstaltung/' . get_query_var( 'event_id' ) );
?>
<aside>
  <nav>
    <div class="item">
      <a>TODO: render content for event with ID <?php echo get_query_var( 'event_id' ); ?></a>
    </div>
  <nav>
</aside>
<main class="detail">
  <section class="note">
    <section class="image" style="background-image: url('<?php echo $image_url; ?>');"></section>
    <h2><a href="$permalink">$pretty->title</a></h2>
    <section class="meta">
      $date, $time$location 
    </section>
    <section class="share">
      <div class="group">
        <label for="permalink">Link zur Veranstaltung</label>
        <div class="formgroup">
          <input type="text" id="permalink" value="<?php echo $permalink; ?>" readonly>
          <button><img src="<?php echo $stylesheet_directory; ?>/img/icons/clipboard-fill.svg" alt="Kopieren"></button>
        </div>
      </div>
      <div class="group">
        <label>Veranstaltung teilen</label>
        <div class="formgroup">
          <button><img src="<?php echo $stylesheet_directory; ?>/img/icons/facebook-fill.svg" alt="Facebook"></button>
          <button><img src="<?php echo $stylesheet_directory; ?>/img/icons/twitter-fill.svg" alt="Twitter"></button>
          <button><img src="<?php echo $stylesheet_directory; ?>/img/icons/telegram-fill.svg" alt="Telegram"></button>
          <button><img src="<?php echo $stylesheet_directory; ?>/img/icons/calendar-event-fill.svg" alt="Kalender"></button>
        </div>
      </div>
    </section>
  </section>
  <article>
    <section class="description">
      $pretty->description
      $event_link
    </section>
  </article>
</main>

<?php get_footer(); ?>
