<?php
/**
 * A helper for getting human readable details out of a Comcal_Event instance.
 *
 * @package GruenesBrett
 */

/**
 * Event prettifier.
 */
class Pretty_Event extends Comcal_Pretty_Event {
    protected function initialize_prettier_map() {
        $map = parent::initialize_prettier_map();

        // Example!
        $map['upperTitle'] = function () {
            return strtoupper( $this->title );
        };
        return $map;
    }
}
