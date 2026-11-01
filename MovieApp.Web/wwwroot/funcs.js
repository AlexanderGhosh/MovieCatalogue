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