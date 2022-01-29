<?php
/**
 * RSS2 Feed
 *
 * @package GruenesBrett
 */

header( 'Content-Type: ' . feed_content_type( 'rss2' ) . '; charset=' . get_option( 'blog_charset' ), true );
echo '<?xml version="1.0" encoding="' . get_option( 'blog_charset' ) . '"?' . ">\n";

$now      = new DateTime( 'now', wp_timezone() );
$category = Common_Data::get_active_category_name();
$name     = get_bloginfo( 'name' );

if ( ! empty( $category ) ) {
    $name .= " | $category";
}

?>
<rss version="2.0"
    xmlns:content="http://purl.org/rss/1.0/modules/content/"
    xmlns:wfw="http://wellformedweb.org/CommentAPI/"
    xmlns:dc="http://purl.org/dc/elements/1.1/"
    xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:sy="http://purl.org/rss/1.0/modules/syndication/"
    xmlns:slash="http://purl.org/rss/1.0/modules/slash/"
>

<channel>
    <title><?php echo $name; ?></title>
    <atom:link href="<?php self_link(); ?>" rel="self" type="application/rss+xml" />
    <link><?php bloginfo_rss( 'url' ); ?></link>
    <description><?php bloginfo_rss( 'description' ); ?></description>
    <lastBuildDate><?php echo $now->format( 'r' ); ?></lastBuildDate>
    <language><?php bloginfo_rss( 'language' ); ?></language>
    <sy:updatePeriod>hourly</sy:updatePeriod>
    <sy:updateFrequency>1</sy:updateFrequency>
    <?php
    if ( verify_community_calendar_loaded( true ) ) {
        echo Event_Feed_Builder::get_instance()->get_html();
    }
    ?>
</channel>
</rss>
