using BlazorVideoPlayer;
using Microsoft.AspNetCore.Components;

namespace MovieApp.Web.Components.Pages
{
    [StreamRendering]
    public partial class MoviePlayer : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }

        private List<Source> Sources => new()
        {
            new()
            {
                Src = $"/api/Video/{MovieId}",
                Type = "video/mp4"
            }
        };
        private void OnEndedVideo()
        {
            Console.WriteLine("End of play");
        }
        private void OnPlayVideo()
        {
            Console.WriteLine("Start playing");
        }
        private void OnVideoTimeUpdate(float currentTime, float duration)
        {
            Console.WriteLine("Current Time: " + currentTime);
            Console.WriteLine("Duration: " + duration);
        }
    }
}
