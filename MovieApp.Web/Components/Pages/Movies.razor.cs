using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieCatalogue.Models;

namespace MovieApp.Web.Components.Pages
{
    [StreamRendering]
    public partial class Movies(IMovieService _movieSvc) : ComponentBase()
    {
        private Movie[] _movies = [];
        public Movie? _selectedMovie;
        protected override async Task OnInitializedAsync()
        {
            _movies = [.. await _movieSvc.GetAllMoviesAsync()];
        }

        private void WatchMovie(Movie movie)
        {
            Navigation.NavigateTo($"/movie/{movie.Id}");
        }
    }
}
