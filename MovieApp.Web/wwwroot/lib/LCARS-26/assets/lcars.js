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
// LCARS keystroke sound (not to be used with hyperlinks)
  const LCARSkeystroke = document.getElementById('LCARSkeystroke');
  const allPlaySoundButtons = document.querySelectorAll('.playSoundButton');
  allPlaySoundButtons.forEach(button => {
    button.addEventListener('click', function() {
      LCARSkeystroke.pause();
      LCARSkeystroke.currentTime = 0; // Reset to the beginning of the sound
      LCARSkeystroke.play();
    });
  });