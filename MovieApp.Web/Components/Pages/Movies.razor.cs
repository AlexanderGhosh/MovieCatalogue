using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MovieApp.Web.Components.Modals;
using MovieApp.Web.Models;
using MovieCatalogue;

namespace MovieApp.Web.Components.Pages
{
    public partial class Movies(IMovieService _movieSvc) : ComponentBase()
    {
        private Movie[] _movies = [];
        private IMovieCollection[] _collections = [];
        private bool _creatingCollection;
        private string _newCollectionName = string.Empty;
        public Movie? _selectedMovie;
        [CascadingParameter]
        public IModalService Modal { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            _movies = await GetMovies();
            _collections = await GetCollections();
        }

        private sealed record HomeItem(IMovieCollection? Collection, Movie? Movie)
        {
            public string Name => Collection?.Name ?? Movie?.Title ?? string.Empty;
        }

        // Collections and movies that are not in a collection, shown together.
        // Movies inside a collection only appear on that collection's page.
        private IEnumerable<HomeItem> Items => _collections
            .Select(c => new HomeItem(c, null))
            .Concat(_movies.Where(m => m.CollectionId is null).Select(m => new HomeItem(null, m)))
            .OrderBy(i => i.Name, StringComparer.CurrentCultureIgnoreCase);

        private int CountIn(IMovieCollection collection) => _movies.Count(m => m.CollectionId == collection.Id);

        private void StartCreateCollection()
        {
            _creatingCollection = true;
            _newCollectionName = string.Empty;
        }

        private void CancelCreateCollection()
        {
            _creatingCollection = false;
            _newCollectionName = string.Empty;
        }

        private async Task OnCollectionKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter") await SaveCollection();
            else if (e.Key == "Escape") CancelCreateCollection();
        }

        private async Task SaveCollection()
        {
            var name = _newCollectionName.Trim();
            if (name.Length > 0)
            {
                await _movieSvc.CreateCollection(name);
                _collections = await GetCollections();
            }
            CancelCreateCollection();
        }

        private async Task AssignCollection(Movie movie, string? value)
        {
            int? collectionId = int.TryParse(value, out var id) ? id : null;
            await _movieSvc.SetMovieCollection(movie.Id, collectionId);
            _movies = await GetMovies();
        }

        private async Task<IMovieCollection[]> GetCollections()
        {
            var collections = await _movieSvc.GetAllCollectionsAsync();
            return [.. collections];
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

        private async Task RenameMovie(Movie movie, string newName)
        {
            if (movie.Title == newName) return;
            await _movieSvc.UpdateMovie<Movie>(movie.Id, m => m.Title = newName);
        }

        private async Task DeleteMovie(Movie movie)
        {
            await _movieSvc.DeleteMovie(movie.Id);
            _movies = await GetMovies();
        }

        private async Task<Movie[]> GetMovies()
        {
            var movies = await _movieSvc.GetAllMoviesAsync();
            return [.. movies.Cast<Movie>()];
        }
    }
}
