<?php
/**
 * Navigation bar for the explore page
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded( true ) ) {
    return;
}

?>
<aside>
  <nav>
    <?php
      echo Category_Provider::get_category_buttons();
    ?>
  </nav>
</aside>
