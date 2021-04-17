<?php
/**
 * Show available categories.
 *
 * @package GruenesBrett
 */

/**
 * Retrieves categories from the database.
 */
class Category_Provider {

    public static function get_all() : array {
        return Comcal_Category::get_all();
    }

    public static function get_category_buttons() : string {
        $html = <<<XML
            <div class="item">
              <a href="?">Alle</a>
            </div>
XML;

        foreach ( static::get_all() as $category ) {
            list(
              $background,
              $foreground
              )   = $category->get_background_foreground_colors();
            $name = $category->get_field( 'name' );

            $bg_style = "background-color: $background;";
            $fg_style = "color: $foreground;";
            $href     = '?category=' . rawurlencode( $name );

            $html .= <<<XML
            <div class="item" style="$bg_style">
              <a href="$href" style="$fg_style">$name</a>
            </div>
    XML;
        }
        return $html;
    }
}
