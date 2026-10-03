using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MovieApp.Web.Models;
using MovieCatalogue;

namespace MovieApp.Web.Components.Pages
{
    public partial class CollectionView(IMovieService _movieSvc, IJSRuntime _js) : ComponentBase()
    {
        [Parameter]
        public int CollectionId { get; set; }

        private IMovieCollection? _collection;
        private Movie[] _movies = [];
        private Movie[] _available = [];
        private bool _loaded;

        private bool _renaming;
        private string _newName = string.Empty;
        private string? _selectedToAdd;

        protected override async Task OnParametersSetAsync()
        {
            _loaded = false;
            await Load();
            _loaded = true;
        }

        private async Task Load()
        {
            _collection = await _movieSvc.GetCollection(CollectionId);
            if (_collection is null)
            {
                _movies = [];
                _available = [];
                return;
            }

            _movies = [.. (await _movieSvc.GetMoviesInCollectionAsync(CollectionId)).Cast<Movie>().OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase)];

            // Movies that aren't in any collection yet can be added from here.
            var all = await _movieSvc.GetAllMoviesAsync();
            _available = [.. all.Cast<Movie>().Where(m => m.CollectionId is null).OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)];
            _selectedToAdd = null;
        }

        private void StartRename()
        {
            _renaming = true;
            _newName = _collection?.Name ?? string.Empty;
        }

        private void CancelRename()
        {
            _renaming = false;
            _newName = string.Empty;
        }

        private async Task OnRenameKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter") await SaveRename();
            else if (e.Key == "Escape") CancelRename();
        }

        private async Task SaveRename()
        {
            var name = _newName.Trim();
            if (name.Length > 0 && name != _collection?.Name)
            {
                await _movieSvc.RenameCollection(CollectionId, name);
                await Load();
            }
            CancelRename();
        }

        private async Task DeleteCollection()
        {
            var ok = await _js.InvokeAsync<bool>("confirm",
                $"Delete the collection \"{_collection?.Name}\"? The movies in it will be kept.");
            if (!ok) return;

            await _movieSvc.DeleteCollection(CollectionId);
            Navigation.NavigateTo("/");
        }

        private async Task RemoveMovie(Movie movie)
        {
            await _movieSvc.SetMovieCollection(movie.Id, null);
            await Load();
        }

        private async Task AddSelectedMovie()
        {
            if (!int.TryParse(_selectedToAdd, out var movieId)) return;
            await _movieSvc.SetMovieCollection(movieId, CollectionId);
            await Load();
        }
    }
}
