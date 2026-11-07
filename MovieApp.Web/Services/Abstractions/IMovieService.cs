using MovieApp.Web.Models;

namespace MovieApp.Web.Services.Abstractions
{
    public interface IMovieService
    {
        Task UploadMovieAsync(Movie movie, MovieContent? content, CancellationToken ct = default);
        ValueTask<Movie?> GetMovie(int movieId, CancellationToken ct = default);
        Task<MovieContent?> GetMovieContent(int movieId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync(CancellationToken ct = default);

        Task UpdateMovie(int movieId, Action<Movie> update, CancellationToken ct = default);

        Task DeleteMovie(int movieId, CancellationToken ct = default);

        Task<IReadOnlyCollection<MovieCollection>> GetAllCollectionsAsync(CancellationToken ct = default);
        ValueTask<MovieCollection?> GetCollection(int collectionId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Movie>> GetMoviesInCollectionAsync(int collectionId, CancellationToken ct = default);
        Task<MovieCollection> CreateCollection(string name, CancellationToken ct = default);
        Task RenameCollection(int collectionId, string name, CancellationToken ct = default);
        Task DeleteCollection(int collectionId, CancellationToken ct = default);
        Task AddMovieToCollection(int movieId, int collectionId, CancellationToken ct = default);
        Task RemoveMovieFromCollection(int movieId, int collectionId, CancellationToken ct = default);
        Task RemoveCollectionFromCollection(int childId, int parentId, CancellationToken ct = default);
        Task AddCollectionToCollection(int parentId, int childId, CancellationToken ct = default);

        Task<Thumbnail?> GetMovieThumbnail(int movieId, CancellationToken ct = default);
        Task SetMovieThumbnail(int movieId, Thumbnail thumbnail, CancellationToken ct = default);
        Task<Thumbnail?> GetCollectionThumbnail(int collectionId, CancellationToken ct = default);
        Task SetCollectionThumbnail(int collectionId, Thumbnail thumbnail, CancellationToken ct = default);
    }
}