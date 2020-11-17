$(function () {

    var $grid = $('.grid').masonry({
        itemSelector: '.grid-item'
    });

    $grid.imagesLoaded().progress(function () {
        $grid.masonry('layout');
    });

});