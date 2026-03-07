using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieApp.Web.Components.Modals;
using MovieApp.Web.Models;

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
            _movies = await GetMovies();
        }

        private async Task AddMovie()
        {
            var resTask = Modal.Show<AddMovieModal>()?.Result;

            if (resTask is null) return;
            var res = await resTask;

            if (res.Cancelled || res.Data is not AddMovie data) return;
            var (movie, content, thumbnail) = data;

            await _movieSvc.UploadMovieAsync(movie, content, thumbnail);
            _movies = await GetMovies();
        }

        private void WatchMovie(Movie movie)
        {
            Navigation.NavigateTo($"/movie/{movie.Id}");
        }

        private async Task<Movie[]> GetMovies()
        {
            var movies = await _movieSvc.GetAllMoviesAsync();
            return [.. movies.Cast<Movie>()];
        }
    }
}
