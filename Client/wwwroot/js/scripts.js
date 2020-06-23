
//$(document).ready(function () {

//});


//function InitSlider() {

//  var checkExist = setInterval(function () {
//    if ($('#mycontentslider').length) {

//      $("#mycontentslider").lightSlider({
//        loop: true,
//        item: 3,
//        keyPress: true
//      });


//      clearInterval(checkExist);
//    }
//  }, 100);

//}

function SetNavBarActive() {
  var url = window.location;
  console.log(url.pathname);

}

function toggleNavBar() {
  if ($("#navBarBtn").is(":visible") == true && $('#navbarSupportedContent').hasClass("show")) {
    $('#navbarSupportedContent').collapse('toggle');
  }
}