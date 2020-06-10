$(document).ready(function () {
  // Activate WOW.js
  new WOW().init();

});

//$(window).ready(function() {
//  // Splash Screen
//  $("#splash").fadeOut();
//});

function SetNavBarActive() {
  var url = window.location;
  console.log(url.pathname);


  //$(".nav a").find(".active").removeClass("active");
  //$(".nav a").parent().addClass("active");

}