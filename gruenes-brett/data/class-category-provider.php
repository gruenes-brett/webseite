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

    public static function get_all() {
        return Comcal_Category::get_all();
    }

    public static function get_background_foreground_colors( Comcal_Category $category ) {
        return comcal_create_unique_colors( $category->get_field( 'name' ) );
    }
}
