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
}
