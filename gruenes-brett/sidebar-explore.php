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
    $all_categories = Category_Provider::get_all();
    foreach ( $all_categories as $category ) {
        list(
          $background,
          $foreground
          )   = Category_Provider::get_background_foreground_colors( $category );
        $name = $category->get_field( 'name' );

        $bg_style = "background-color: $background;";
        $fg_style = "color: $foreground;";

        echo <<<XML
        <div class="item" style="$bg_style">
          <a href="" style="$fg_style">$name</a>
        </div>
XML;
    }
    ?>
  </nav>
</aside>
