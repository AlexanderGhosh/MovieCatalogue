using Microsoft.AspNetCore.Components;

namespace MovieApp.Web.Components.Pages
{
    [StreamRendering]
    public partial class MoviePlayer : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }
    }
}
