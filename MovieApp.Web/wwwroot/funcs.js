window.audioPlayer = {
    playByUrl: function (url) {
        const audio = new Audio(url);
        audio.play().catch(err => console.error("Audio play failed:", err.name, err.message));
    }
};