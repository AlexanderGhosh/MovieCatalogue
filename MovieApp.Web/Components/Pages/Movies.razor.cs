using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MovieApp.Web.Components.Layout;
using MovieApp.Web.Components.Modals;
using MovieApp.Web.Models;
using MovieApp.Web.Services.Abstractions;

namespace MovieApp.Web.Components.Pages
{
    public enum SortOptions
    {
        Default,
        CollectionsOnly,
        MoviesOnly,
        AddedOn,
        Name
    }
    public partial class Movies(IMovieService _movieSvc) : ComponentBase()
    {
        private Movie[] _movies = [];
        private MovieCollection[] _collections = [];
        private bool _creatingCollection;
        private string _newCollectionName = string.Empty;
        [CascadingParameter]
        public IModalService Modal { get; set; } = default!;
        [CascadingParameter]
        public MainLayout Layout { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _movies = await GetMovies();
            _collections = await GetCollections();
            Layout.OnClickButton1 = _ => _sortOptions = SortOptions.Default;
            Layout.OnClickButton2 = _ => _sortOptions = SortOptions.Name;
            Layout.OnClickButton3 = _ => _sortOptions = SortOptions.AddedOn;
        }

        private SortOptions _sortOptions = SortOptions.Default;

        private IEnumerable<IMovieItem> Items
        {
            get
            {
                var topLevelCollections = (IEnumerable<IMovieItem>)_collections.Where(x => x.ParentCollections.Count == 0);
                var topLevelMovies = _movies.Where(x => x.Collections.Count == 0);
                var defaultSort = topLevelCollections.Concat(topLevelMovies);
                return _sortOptions switch
                {
                    SortOptions.Default => defaultSort,
                    SortOptions.CollectionsOnly => topLevelCollections,
                    SortOptions.MoviesOnly => topLevelMovies,
                    SortOptions.AddedOn => defaultSort.OrderBy(x => x.AddedOn),
                    SortOptions.Name => defaultSort.OrderBy(x => x.Name),
                    _ => defaultSort,
                };
            }
        }

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

        private async Task<MovieCollection[]> GetCollections()
        {
            var collections = await _movieSvc.GetAllCollectionsAsync();
            return [.. collections];
        }

        private async Task AddMovie()
        {
            var options = new ModalOptions()
            {
                Class = "lcars-modal",
                Position = ModalPosition.TopCenter
            };
            var resTask = Modal.Show<AddMovieModal>(options)?.Result;

            if (resTask is null) return;
            var res = await resTask;

            if (res.Cancelled || res.Data is not AddMovieModal.AddMovie data) return;
            var (movie, content) = data;

            await _movieSvc.UploadMovieAsync(movie, content);
            _movies = await GetMovies();
        }

        private async Task DeleteMovie(Movie movie)
        {
            await _movieSvc.DeleteMovie(movie.Id);
            _movies = await GetMovies();
            StateHasChanged();
        }

        private async Task<Movie[]> GetMovies()
        {
            var movies = await _movieSvc.GetAllMoviesAsync();
            return [.. movies.Cast<Movie>()];
        }
    }
}
