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
    var ZONE = 150;        // px from edge where scrolling starts
    var MIN_SPEED = 2;     // px per tick at the zone boundary
    var MAX_SPEED = 30;    // px per tick at the very edge, fully ramped
    var RAMP_MS = 1000;    // time to reach full speed while held in the zone

    var direction = 0;     // -1 up, 1 down, 0 stopped
    var depth = 0;         // 0..1, how far into the zone the pointer is
    var holdStart = 0;     // when the pointer entered the zone
    var timer = null;

    function tick() {
        if (direction === 0) { timer = null; return; }

        var held = Math.min((performance.now() - holdStart) / RAMP_MS, 1);
        var speed = MIN_SPEED + (MAX_SPEED - MIN_SPEED) * depth * (0.3 + 0.7 * held);

        window.scrollBy(0, direction * speed);
        timer = setTimeout(tick, 16);
    }

    function setScroll(dir, d) {
        if (dir !== direction) holdStart = performance.now();
        direction = dir;
        depth = d;
        if (dir !== 0 && timer === null) tick();
    }

    document.addEventListener("drag", function (e) {
        var y = e.clientY;
        // Some browsers fire a final drag event with clientY = 0 on release
        if (y === 0) { setScroll(0, 0); return; }

        if (y < ZONE) {
            setScroll(-1, (ZONE - y) / ZONE);
        } else if (y > window.innerHeight - ZONE) {
            setScroll(1, (y - (window.innerHeight - ZONE)) / ZONE);
        } else {
            setScroll(0, 0);
        }
    });

    document.addEventListener("dragend", function () { setScroll(0, 0); });
    document.addEventListener("drop", function () { setScroll(0, 0); });
})();