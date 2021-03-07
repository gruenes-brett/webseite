<?php
/**
 * Default navigation bar
 *
 * @package GruenesBrett
 */

?>
<aside>
<?php
$output = wp_page_menu(
    array(
        'echo'      => 0,
        'depth'     => 1,
        'container' => 'nav',
        'before'    => '',
        'after'     => '',
    )
);

if ( is_page() ) {

    $current_id = $post->ID;

    if ( $post->post_parent ) {
        $current_id = $post->post_parent;
    }

    $children = wp_page_menu(
        array(
            'echo'      => 0,
            'child_of'  => $current_id,
            'container' => 'nav',
            'before'    => '',
            'after'     => '',
        )
    );

    if ( $children ) {
        $output = wp_page_menu(
            array(
                'echo'      => 0,
                'child_of'  => $current_id,
                'container' => 'nav',
                'before'    => '',
                'after'     => '',
            )
        );
    }
}

// TODO: replace with custom page walker in the wp_page_menu calls!
echo wp_kses_post( preg_replace( '/<li\s(.+?)>(.+?)<\/li>/is', '<div class="item">$2</div>', $output ) );

?>
</aside>
