using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieCatalogue.Models;

namespace MovieApp.Web.Components.Pages
{
    public partial class Movies(IMovieService _movieSvc) : ComponentBase()
    {
        private Movie[] _movies = [];
        public Movie? _selectedMovie; 
        [CascadingParameter] 
        public IModalService Modal { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            var movies = await _movieSvc.GetAllMoviesAsync();
            _movies = [.. movies.Cast<Movie>()];
        }

        private void WatchMovie(Movie movie)
        {
            Navigation.NavigateTo($"/movie/{movie.Id}");
        }
    }
}
