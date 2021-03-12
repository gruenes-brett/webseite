<?php
/**
 * Template Name: Kalender
 *
 * @package GruenesBrett
 */

?>
<?php get_header(); ?>
<?php get_sidebar( 'calendar' ); ?>

<main class="calendar">

<?php

if ( verify_community_calendar_loaded( true ) ) {
    echo Calendar_Table_Builder::create_tables();
    echo comcal_getShowEventBox();
    echo comcal_getEditForm( 'gruenes-brett' );
    if ( comcal_currentUserCanSetPublic() ) {
        echo comcal_getEditCategoriesDialog();
    }
    echo comcal_floatingButtons_func( array( 'addEvent' => true ) );
}
?>

<!-- <table>
    <tr>
      <td><a href="#november-2020" name="november-2020">November 2020</a></td>
      <td>1</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>2</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>3</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>4</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>5</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>6</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>7</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>8</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>9</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>10</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>11</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>12</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>13</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>14</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>15</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>16</td>
      <td>Mo</td>
      <td>
        <article>
        <h2><a href="">Umweltringvorlesung „Grüner Hedonimus – ist eine lebenswerte Welt tanzbar?!“</a></h2>
        <section class="meta">
          16.11.2020, 16:40-18:10
        </section>
      </article>
      <article>
        <h2><a href="">Klimaverträgliches Reisen – aber wie?! Von Titos Gebirgsbahn bis zum Trekking auf dem Lykischen Weg</a></h2>
        <section class="meta">
          16.11.2020, 18:00-19:30, Schloßstraße 2
        </section>
      </article>
      <article>
        <h2><a href="detail.html .detail" data-featherlight="ajax">BUNDjugend Treffen</a></h2>
        <section class="meta">
          16.11.2020, 19:00-21:00, Kamenzer Straße 35
        </section>
      </article>
    </td>
    </tr>
    <tr>
      <td></td>
      <td>17</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr class="today">
      <td></td>
      <td>18</td>
      <td>Mi</td>
      <td>
        <article>
          <h2><a href="">Praktischer Naturschutz BUND: Geräte einwintern</a></h2>
          <section class="meta">
            18.11.2020, Podemuser Ring 1
          </section>
        </article>
      </td>
    </tr>
    <tr>
      <td></td>
      <td>19</td>
      <td>Do</td>
      <td>
        <article>
          <h2><a href="">RepairCafé (Dresden)</a></h2>
          <section class="meta">
            19.11.2020, 18:00-21:00, Bürgerstr. 68
          </section>
        </article>
      </td>
    </tr>
    <tr>
      <td></td>
      <td>20</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>21</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>22</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>23</td>
      <td>Mo</td>
      <td>
        <article>
          <h2><a href="">Umweltringvorlesung „Grüner Hedonismus – ist eine lebenswerte Welt tanzbar?!“</a></h2>
          <section class="meta">
            23.11.2020, 16:40-18:10
          </section>
        </article>
        <article>
          <h2><a href="">BUNDjugend Treffen</a></h2>
          <section class="meta">
            23.11.2020, 19:00-21:00
          </section>
        </article>
      </td>
    </tr>
    <tr>
      <td></td>
      <td>24</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>25</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>26</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>27</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>28</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>29</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>30</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td><a href="#dezember-2020" name="dezember-2020">Dezember 2020</a></td>
      <td>1</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>2</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>3</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>4</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>5</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>6</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>7</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>8</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>9</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>10</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>11</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>12</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>13</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>14</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>15</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>16</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>17</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>18</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>19</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>20</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>21</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>22</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>23</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>24</td>
      <td>Do</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>25</td>
      <td>Fr</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>26</td>
      <td>Sa</td>
      <td></td>
    </tr>
    <tr class="weekend">
      <td></td>
      <td>27</td>
      <td>So</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>28</td>
      <td>Mo</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>29</td>
      <td>Di</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>30</td>
      <td>Mi</td>
      <td></td>
    </tr>
    <tr>
      <td></td>
      <td>31</td>
      <td>Do</td>
      <td></td>
    </tr>
  </table> -->
</main>
<?php get_footer(); ?>
