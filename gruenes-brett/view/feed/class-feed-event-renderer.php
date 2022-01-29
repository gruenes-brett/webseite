<?php
/**
 * Class definition for Feed_Event_Renderer.
 *
 * @package GruenesBrett
 */

if ( ! verify_community_calendar_loaded() ) {
    return;
}

/**
 * Defines how an event is rendered in the RSS feed.
 */
class Feed_Event_Renderer extends Comcal_Default_Event_Renderer {
    public function render( Comcal_Event $event, int $day ) : string {
        if ( 0 !== $day ) {
            // Render multi-day events only once.
            return '';
        }
        $pretty = new Pretty_Event( $event );

        // this hack is needed to throw away the wrong timezone data.
        $created_date = new DateTime( $event->get_created_date()->format( 'D, d M Y H:i:s' ), wp_timezone() );
        $created      = $created_date->format( 'r' );
        $categories   = '';

        foreach ( $event->get_categories() as $category ) {
            $public_fields = $category->get_public_fields();
            $name          = $public_fields['name'];
            $categories   .= "<category>$name</category>";
        }

        return <<<XML

        <item>
            <title>$pretty->name</title>
            <link>$pretty->permalink</link>
            <dc:creator><![CDATA[$pretty->organizer]]></dc:creator>
            <pubDate>$created</pubDate>
            <guid isPermaLink="false">$pretty->event_id</guid>
            <description><![CDATA[$pretty->meta<br><br>$pretty->clickable_description]]></description>
            $categories
        </item>
        XML;
    }
}

Event_Popup::verify_popup_initialized();
