$(document).ready(function () {
  // Activate WOW.js
  new WOW().init();

});

$(window).ready(function() {
  // Splash Screen
  $("#splash").fadeOut();
});


//$('nav ul li a').click(function () { $('li a').removeClass("active"); $(this).addClass("active"); });

function SetNavBarActive() {
  var url = window.location;
  //console.log(url.pathname);

  //$(".nav a").find(".active").removeClass("active");
  //$(".nav a").parent().addClass("active");

}

/*
    Carousel
*/
$('#MainSlider').on('slide.bs.carousel', function (e) {
  /*
      CC 2.0 License Iatek LLC 2018 - Attribution required
  */
  var $e = $(e.relatedTarget);
  var idx = $e.index();
  var itemsPerSlide = 4;
  var totalItems = $('.carousel-item').length;

  if (idx >= totalItems - (itemsPerSlide - 1)) {
    var it = itemsPerSlide - (totalItems - idx);
    for (var i = 0; i < it; i++) {
      // append slides to end
      if (e.direction == "left") {
        $('.carousel-item').eq(i).appendTo('.carousel-inner');
      }
      else {
        $('.carousel-item').eq(0).appendTo('.carousel-inner');
      }
    }
  }
});