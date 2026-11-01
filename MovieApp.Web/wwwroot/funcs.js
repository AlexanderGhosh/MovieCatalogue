window.audioPlayer = {
    playByUrl: function (url) {
        const audio = new Audio(url);
        audio.play().catch(err => console.error("Audio play failed:", err.name, err.message));
    }
};

window.fileHelper = {
    downloadByUrl: function (url, fileName) {
        const anchor = document.createElement("a");
        anchor.href = url;
        anchor.download = fileName ?? "";
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();
    },

    downloadFromStream: async function (fileName, streamRef) {
        const buffer = await streamRef.arrayBuffer();
        const blobUrl = URL.createObjectURL(new Blob([buffer]));
        const anchor = document.createElement("a");
        anchor.href = blobUrl;
        anchor.download = fileName ?? "";
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();
        URL.revokeObjectURL(blobUrl);
    }
};

window.videoInterop = {
    fastSeek: (video, seconds) => {
        if (!video || seconds <= 0) return;

        const seek = () => {
            if (typeof video.fastSeek === "function") {
                video.fastSeek(seconds);
            } else {
                // fastSeek isn't supported in Chrome/Edge, so fall back
                video.currentTime = seconds;
            }
        };

        if (video.readyState >= 1) {
            seek(); // metadata already loaded
        } else {
            video.addEventListener("loadedmetadata", seek, { once: true });
        }
    }
};

window.videoInterop.whenReady = (video) => new Promise((resolve) => {
    if (!video) return resolve(false);
    if (video.readyState >= 1) return resolve(true); // HAVE_METADATA

    const done = (ok) => {
        video.removeEventListener("loadedmetadata", onLoaded);
        video.removeEventListener("error", onError);
        resolve(ok);
    };
    const onLoaded = () => done(true);
    const onError = () => done(false);

    video.addEventListener("loadedmetadata", onLoaded);
    video.addEventListener("error", onError);
});

window.videoInterop.getCurrentTime = async (video) => {
    if (!(await window.videoInterop.whenReady(video))) return 0;
    return Number.isFinite(video.currentTime) ? video.currentTime : 0;
};

window.videoInterop.getDuration = async (video) => {
    if (!(await window.videoInterop.whenReady(video))) return 0;
    return Number.isFinite(video.duration) ? video.duration : 0;
};

(function () {
    var scrollStep = 0;
    var timer = null;

    function tick() {
        if (scrollStep === 0) { timer = null; return; }
        window.scrollBy(0, scrollStep);
        timer = setTimeout(tick, 20);
    }

    function setStep(step) {
        scrollStep = step;
        if (step !== 0 && timer === null) tick();
    }

    document.addEventListener("drag", function (e) {
        var y = e.clientY;
        // Some browsers fire a final drag event with clientY = 0 on release
        if (y === 0) { setStep(0); return; }

        if (y < 150) setStep(-10);
        else if (y > window.innerHeight - 150) setStep(10);
        else setStep(0);
    });

    document.addEventListener("dragend", function () { setStep(0); });
    document.addEventListener("drop", function () { setStep(0); });
})();