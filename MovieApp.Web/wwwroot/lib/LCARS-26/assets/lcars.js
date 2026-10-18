// Modified 2026 Jun 20

document.addEventListener("touchstart", function() {},false);
function playSoundAndRedirect(audioId, url) {
    var audio = document.getElementById(audioId);
    audio.play();

    audio.onended = function() {
        window.location.href = url;
    };
}
function goToAnchor(anchorId) {
    window.location.hash = anchorId;
}
function redirect(url) {
    if (location.pathname != '/')
        window.location.href = url;
}

function playSound(audioId) {
    document.getElementById(audioId).play();
}
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