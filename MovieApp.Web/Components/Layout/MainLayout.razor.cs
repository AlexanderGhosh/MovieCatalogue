using MovieApp.Web.Services.Abstractions;

namespace MovieApp.Web.Components.Layout
{
    public partial class MainLayout(IAudioPlayer audioPlayer)
    {
        public async Task PlayRandomAudio()
        {
            var rand = Random.Shared.Next(0, audioPlayer.Count);
            await audioPlayer.PlayAudio(rand);
        }

        public async Task PlayAudio(int id)
        {
            await audioPlayer.PlayAudio(id);
        }
    }
}
