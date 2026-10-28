// Modified 2026 Jun 20

document.addEventListener("touchstart", function () { }, false);

// Accordion drop-down
var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
  acc[i].addEventListener("click", function() {
    this.classList.toggle("active");
    var accordionContent = this.nextElementSibling;
    if (accordionContent.style.maxHeight){
      accordionContent.style.maxHeight = null;
    } else {
      accordionContent.style.maxHeight = accordionContent.scrollHeight + "px";
    } 
  });
}