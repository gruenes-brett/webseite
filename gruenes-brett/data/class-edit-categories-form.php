<?php
/**
 * Extends the form with some color pickers and customized styling.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Edit Categories Form
 */
class Edit_Categories_Form extends Comcal_Edit_Categories_Form {

    /**
     * Specifies the action name.
     *
     * @var string
     */
    protected static $action_name = 'submit_categories_data';

    protected function get_category_form( Comcal_Category $category, int $index ) {
        $suffix      = "_$index";
        $name        = $category->get_field( 'name' );
        $category_id = $category->get_field( 'categoryId' );
        $style_form  = $this->get_style_form( $category, $suffix );
        return <<<XML
            <table class="">
                <tr>
                    <td>
                        <input name="categoryId$suffix" id="categoryId$suffix" value="$category_id" type="hidden">
                        <div class="formgroup">
                            <div class="row">
                                <label for="categoryName$suffix">Name</label>
                                <input type="text" name="name$suffix" id="categoryName$suffix" placeholder="" value="$name" required>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        $style_form
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="formgroup highlight">
                            <div class="row">
                                <input type="checkbox" name="delete$suffix" id="categoryDelete$suffix" value="$category_id" unchecked>
                                <label for="categoryDelete$suffix">Kategorie löschen?</label>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
XML;
    }

    protected function get_style_form( $category, string $suffix ) {
        $style = $category->get_field( 'style' );
        if ( ! $style ) {
            $style = $this->generate_style( $category->get_field( 'name' ) );
        }
        $styles = explode( ',', $style );
        return <<<XML
            <input type="hidden" name="style$suffix" id="categoryStyle$suffix" value="$style" />
            <div class="formgroup">
                <div class="row">
                    <label for="categoryTextStyle$suffix">Text</label>
                    <input type="text"
                           name="categoryTextStyle$suffix"
                           id="categoryTextStyle$suffix"
                           class="coloris"
                           data-action="categories#colorUpdated"
                           data-update="categoryStyle$suffix"
                           data-role="text"
                           placeholder="#fff"
                           value="$styles[1]">
                </div>
                <div class="row">
                    <label for="categoryBackgroundStyle$suffix">Hintergrund</label>
                    <input type="text"
                           name="categoryBackgroundStyle$suffix"
                           id="categoryBackgroundStyle$suffix"
                           class="coloris"
                           data-action="categories#colorUpdated"
                           data-update="categoryStyle$suffix"
                           data-role="background"
                           placeholder="#000"
                           value="$styles[0]">
                </div>
            </div>
XML;
    }

    protected function get_add_category_form() {
        $suffix = '_new';
        return <<<XML
            <table>
                <tr>
                    <td>
                        <label for="categoryName$suffix">Neue Kategorie anlegen</label>
                        <input type="text" name="name$suffix" id="categoryName$suffix" placeholder="Neuer Kategoriename" value="">
                    </td>
                </tr>
            </table>
XML;
    }
}

Edit_Categories_Form::register_form();
