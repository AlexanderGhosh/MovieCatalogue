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