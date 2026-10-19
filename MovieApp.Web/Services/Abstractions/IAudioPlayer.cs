// C#
using MovieApp.Web.Services.Implementations;

namespace MovieApp.Web.Services.Abstractions
{
    public interface IAudioPlayer
    {
        Task LoadAudio();
        Task<int> PlayAudio(int audioId);
        Task StopAudio(int playId);
        Task<IEnumerable<AudioFileInfo>> GetAllAudioFiles();
        int Count { get; }
    }
}