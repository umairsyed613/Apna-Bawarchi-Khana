
$(document).ready(function () {
  setTimeout(function () {
    $("#mycontentslider").lightSlider({
      loop: true,
      item: 3,
      keyPress: true
    });
  }, 3000);

});


function SetNavBarActive() {
  var url = window.location;
  console.log(url.pathname);

}

function toggleNavBar() {
  if ($("#navBarBtn").is(":visible") == true && $('#navbarSupportedContent').hasClass("show")) {
    $('#navbarSupportedContent').collapse('toggle');
  }
}