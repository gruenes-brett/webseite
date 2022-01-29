<?php
/**
 * Show available categories.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Retrieves categories from the database.
 */
class Category_Provider {

    public static function get_all() : array {
        return Comcal_Category::get_all();
    }

    public static function get_category_names() : array {
        return array_map( fn( $category ) => $category->get_field( 'name' ), static::get_all() );
    }

    public static function get_category_buttons() : string {
        $html = <<<XML
            <div class="item">
              <a href="?">Alle</a>
            </div>
XML;

        $active_category_name = Common_Data::get_active_category_name();

        foreach ( static::get_all() as $category ) {
            list(
              $background,
              $foreground
              )   = $category->get_background_foreground_colors();
            $name = $category->get_field( 'name' );

            $bg_style     = "background-color: $background;";
            $fg_style     = "color: $foreground;";
            $href         = '?category=' . rawurlencode( $name );
            $active_class = $active_category_name === $name ? 'active' : '';

            $html .= <<<XML
            <div class="item $active_class" style="$bg_style">
              <a href="$href" style="$fg_style">$name</a>
            </div>
    XML;
        }
        return $html;
    }
}
