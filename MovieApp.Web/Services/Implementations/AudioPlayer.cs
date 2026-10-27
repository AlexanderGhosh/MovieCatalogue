using Microsoft.JSInterop;
using MovieApp.Web.Services.Abstractions;
using System.Collections.Concurrent;

namespace MovieApp.Web.Services.Implementations
{
    public record AudioFileInfo(int Id, string Name, string Url, long Length);

    public class AudioPlayer(IJSRuntime jsRuntime, IWebHostEnvironment env) : IAudioPlayer
    {
        private record Audio(int Id, string Url, bool Playing)
        {
            public override int GetHashCode() => Id;
        }

        private static int _playCounter;
        private static int _idCounter;
        private readonly ConcurrentDictionary<int, Audio> _audioElements = new();
        private readonly List<KeyValuePair<int, Audio>> _audioPlays = new();
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly IWebHostEnvironment _env = env;
        private bool _loaded = false;

        // Scans wwwroot/audio for .mp3 and .wav files (top-level only) and registers them.
        public async Task LoadAudio()
        {
            _audioElements.Clear();

            const string audioFolder = "lib/LCARS-26/assets"; // wwwroot/audio
            var provider = _env.WebRootFileProvider;
            var dirContents = provider.GetDirectoryContents(audioFolder);

            if (!dirContents.Exists) return;

            foreach (var file in dirContents.Where(f => !f.IsDirectory))
            {
                var ext = Path.GetExtension(file.Name);
                if (!ext.Equals(".mp3", StringComparison.OrdinalIgnoreCase) &&
                    !ext.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var id = Interlocked.Increment(ref _idCounter);
                // URL the browser can fetch (Static Files must be enabled)
                var url = "/" + Path.Combine(audioFolder, file.Name).Replace('\\', '/');
                var audio = new Audio(id, url, false);
                _audioElements.TryAdd(id, audio);
            }
        }

        public async Task<int> PlayAudio(int audioId)
        {
            await TryLoad();
            if (!_audioElements.TryGetValue(audioId, out var audio)) return -1;

            // JS helper should create/play an <audio> element by URL
            await _jsRuntime.InvokeVoidAsync("audioPlayer.playByUrl", audio.Url);

            var playId = Interlocked.Increment(ref _playCounter);
            _audioPlays.Add(new(playId, audio));
            return playId;
        }

        public async Task<int> PlayRandomAudio()
        {
            await TryLoad();
            var rand = Random.Shared.Next(0, _audioElements.Count);
            return await PlayAudio(rand);
        }

        public async Task StopAudio(int playId)
        {
            await TryLoad();
            // Implement stop via JS interop (e.g., audioPlayer.stop(playId)).
            throw new NotImplementedException();
        }

        // Optional helper to expose list to callers
        public async Task<IEnumerable<AudioFileInfo>> GetAllAudioFiles()
        {
            await TryLoad();
            return _audioElements.Values.Select(a => new AudioFileInfo(a.Id, Path.GetFileName(a.Url), a.Url, 0));
        }

        private async Task TryLoad()
        {
            if (!_loaded)
                await LoadAudio();
            _loaded = true;
        }
    }
}