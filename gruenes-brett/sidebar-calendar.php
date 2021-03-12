<?php
/**
 * Navigation bar for the calendar page
 *
 * @package GruenesBrett
 */

?>
<aside>
  <nav>

    <?php
    $month_links = Calendar_Table_Builder::get_instance()->get_month_links();
    foreach ( $month_links as $name => $link_name ) :
        ?>

    <div class="item">
      <a href="#<?php echo $link_name; ?>"><?php echo $name; ?></a>
    </div>

    <?php endforeach ?>
  </nav>
</aside>
