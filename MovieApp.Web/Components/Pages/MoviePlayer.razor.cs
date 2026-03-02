using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Models;

namespace MovieApp.Web.Components.Pages
{
    [StreamRendering]
    public partial class MoviePlayer : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }
    }
}
