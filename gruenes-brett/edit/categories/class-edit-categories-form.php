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
                <tr>
                    <td>
                        <input name="categoryId$suffix" id="categoryId$suffix" value="$category_id" type="hidden">
                        <label for="categoryName$suffix">Name der Kategorie</label>
                    </td>
                    <td>
                        <input type="text" name="name$suffix" id="categoryName$suffix" placeholder="" value="$name" required>
                    </td>
                </tr>
                $style_form
                <tr>
                    <td></td>
                    <td>
                        <div class="formgroup highlight">
                            <div class="row">
                                <input type="checkbox" name="delete$suffix" id="categoryDelete$suffix" value="$category_id" unchecked>
                                <label for="categoryDelete$suffix">Kategorie löschen?</label>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
XML;
    }

    protected function get_style_form( $category, string $suffix ) {
        $style = $category->get_field( 'style' );
        if ( ! $style ) {
            $style = $this->generate_style( $category->get_field( 'name' ) );
        }
        $styles = explode( ',', $style );
        return <<<XML
        <tr>
            <td>
                <input type="hidden" name="style$suffix" id="categoryStyle$suffix" value="$style" />
                <label for="categoryTextStyle$suffix">Schriftfarbe</label>
            </td>
            <td>
                <input type="text"
                    name="categoryTextStyle$suffix"
                    id="categoryTextStyle$suffix"
                    class="coloris"
                    data-action="categories#colorUpdated"
                    data-update="categoryStyle$suffix"
                    data-role="text"
                    placeholder="#fff"
                    value="$styles[1]">
            </td>
        </tr>
        <tr>
            <td>
                <label for="categoryBackgroundStyle$suffix">Hintergrundfarbe</label>
            </td>
            <td>
                <input type="text"
                    name="categoryBackgroundStyle$suffix"
                    id="categoryBackgroundStyle$suffix"
                    class="coloris"
                    data-action="categories#colorUpdated"
                    data-update="categoryStyle$suffix"
                    data-role="background"
                    placeholder="#000"
                    value="$styles[0]">
            </td>
        </tr>
XML;
    }

    protected function get_add_category_form() {
        $suffix = '_new';
        return <<<XML
            </table>
        </section>
        <section class="note">
            <table>
                <tr>
                    <td>
                        <label for="categoryName$suffix">Neue Kategorie anlegen</label>
                        <input type="text" name="name$suffix" id="categoryName$suffix" placeholder="Neuer Kategoriename" value="">
                    </td>
                </tr>
XML;
    }

    protected function get_form_fields(): string {
        $forms = '<table>';
        $index = 0;
        foreach ( $this->categories as $cat ) {
            $forms .= $this->get_category_form( $cat, $index );
            $index++;
        }
        $add_category_form = $this->get_add_category_form();
        return <<<XML
            $forms
            $add_category_form
            <tr>
                <td></td>
            </tr>
            <tr>
                <td>
                    <button type="submit">Kategorien aktualisieren</button>
                </td>
            </tr>
        </table>
XML;
    }
}

Edit_Categories_Form::register_form();
