<?php
/**
 * Navigation bar for the explore page
 *
 * @package GruenesBrett
 */

?>
<aside>
  <nav>
    <?php
      echo Category_Provider::get_category_buttons();
    ?>
  </nav>
</aside>
