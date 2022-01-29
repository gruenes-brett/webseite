<?php
/**
 * Navigation bar for the not found page
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded( true ) ) {
    return;
}

?>
<aside>
  <nav>
    <div class="item">
      <a href="<?php echo esc_url( get_home_url() ); ?>">zur Startseite</a>
    </div>
  </nav>
</aside>
