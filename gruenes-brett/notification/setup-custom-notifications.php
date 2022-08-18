<?php
/**
 * Define custom email notification messages and registration messages.
 *
 * @package GruenesBrett
 */

/**
 * Custom message on the user registration page.
 */
function gb_register_user_message() {
    $html = '
        <div class="login-infobox">
            <p>
                Bitte gib einen Benutzernamen und eine E-Mail-Adresse für Dich bzw. Deine Initiative an.
            </p>
            <p>
                Nach einer Prüfung werden wir Dich freischalten, so dass selbstständig
                Veranstaltungen eingetragen werden können.
            </p>
        </div>';
    echo $html;
}
add_action( 'register_form', 'gb_register_user_message' );

/**
 * Custom message when the user has submitted registration data.
 *
 * @param string $message The default message.
 */
function gb_post_register_user_message( $message ) {
    if ( isset( $_GET['checkemail'] ) && 'registered' === $_GET['checkemail'] ) {
        $html = '
        <p class="message register">
            Es wurde ein Benutzer für Dich angelegt.
            Nicht wundern, aktuell sind Deine Berechtigungen noch eingeschränkt.
            Wir vom Grünen Brett melden uns bei Dir in einer persönlichen Mail,
            sobald wir Deinen Account überprüft haben. Danke Dir!
        </p>';
        return $html;
    }
    return $message;
}
add_filter( 'login_message', 'gb_post_register_user_message' );

/**
 * Custom email message for the newly registered user.
 *
 * @param array   $content {
 *     Used to build wp_mail().
 *
 *     @type string $to      The intended recipient - New user email address.
 *     @type string $subject The subject of the email.
 *     @type string $message The body of the email.
 *     @type string $headers The headers of the email.
 * }
 * @param WP_User $user     User object for new user.
 * @param string  $blogname The site title.
 *
 * @return array Modified content.
 */
function gb_new_user_notification_email( $content, $user, $blogname ) {
    $username           = $user->data->display_name;
    $content['message'] = "Hallo $username,

Herzlich Willkommen auf unserer Webseite $blogname!

Nicht wundern, aktuell sind Deine Berechtigungen noch eingeschränkt.
Wir vom Grünen Brett melden uns bei Dir in einer persönlichen Mail,
sobald wir Deinen Account überprüft haben. Danke Dir!

Bevor Du Dich einloggen kannst, musst Du unter folgendem Link ein Passwort anlegen.

" . $content['message'];
    return $content;
}
add_filter( 'wp_new_user_notification_email', 'gb_new_user_notification_email', 10, 3 );

/**
 * Custom email message to the admins for the newly registered user.
 *
 * @param array   $content {
 *     Used to build wp_mail().
 *
 *     @type string $to      The intended recipient - New user email address.
 *     @type string $subject The subject of the email.
 *     @type string $message The body of the email.
 *     @type string $headers The headers of the email.
 * }
 * @param WP_User $user     User object for new user.
 * @param string  $blogname The site title.
 *
 * @return array Modified content.
 */
function gb_new_user_notification_email_admin( $content, $user, $blogname ) {
    $username           = $user->data->display_name;
    $email              = $user->data->user_email;
    $user_edit_link     = network_site_url( "wp-admin/user-edit.php?user_id={$user->data->ID}" );
    $content['message'] = "Neue Benutzerregistrierung auf deiner Website $blogname:

Benutzername: $username
E-Mail: $email

Falls der Account nicht von Dir selbst angelegt wurde, ist dies eine Initiative, die sich über die
Website einen Account angelegt hat. Überprüfe, ob die Initiative den Werten des Grünen Brettes
entspricht und vertrauenswürdig ist. Ist dies der Fall, kannst Du dem Account die Rolle „Autor“
geben: $user_edit_link

Der Initiative kannst Du in einer Mail das Dokument mit den Hinweisen für Autor:innen

https://1drv.ms/b/s!AqOfJ2FN2HtVhUQJwF7cR3utRkt4

zusenden.

Ein Textvorschlag hierzu ist:

Liebe:r …,

wir freuen uns sehr, dass wir Euch einen Account einrichten konnten
und ich habe für den Account soeben die notwendigen Berechtigungen eingestellt.

Weil mit dem Account auch eine gewisse „Macht“ über das Grüne Brett einhergeht,
hänge ich dieser Mail Hinweise zur Nutzung des Accounts an.

Hier geht es auch um ganz praktische Dinge wie das Kopieren, Bearbeiten oder
Löschen von Veranstaltungen. Es wäre ideal, wenn alle, die den Account nutzen,
die Hinweise vorher gelesen haben. Meldet Euch gerne, wenn Fragen aufkommen!

Liebe Grüße …
";
    return $content;
}
add_filter( 'wp_new_user_notification_email_admin', 'gb_new_user_notification_email_admin', 10, 3 );

/**
 * Custom logo on the user registration and login page.
 */
function gb_login_logo() {
    ?>
    <style type="text/css">
        #login h1 a, .login h1 a {
            background-image: url(<?php echo get_logo_url(); ?>);
        }
    </style>
    <link rel="icon" href="<?php echo get_image_url( 'favicon.png' ); ?>">
    <link rel="shortcut icon" href="<?php echo get_image_url( 'favicon.png' ); ?>">
    <?php
}

add_action( 'login_enqueue_scripts', 'gb_login_logo' );
add_action( 'login_enqueue_scripts', 'gruenes_brett_styles' );
add_filter(
    'login_headerurl',
    function() {
        return esc_url( get_home_url() );
    }
);
