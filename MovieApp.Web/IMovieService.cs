using BlobStorage;
using MovieApp.Web.Models;

namespace MovieApp.Web
{
    public interface IMovieService
    {
        Task UploadMovieAsync(Movie movie, MovieContent? content);
        ValueTask<Movie?> GetMovie(int movieId);
        Task<MovieContent?> GetMovieContent(int movieId);
        Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync();

        Task UpdateMovie(int movieId, Action<Movie> update);

        Task DeleteMovie(int movieId);

        Task<IReadOnlyCollection<MovieCollection>> GetAllCollectionsAsync();
        ValueTask<MovieCollection?> GetCollection(int collectionId);
        Task<IReadOnlyCollection<Movie>> GetMoviesInCollectionAsync(int collectionId);
        Task<MovieCollection> CreateCollection(string name);
        Task RenameCollection(int collectionId, string name);
        Task DeleteCollection(int collectionId);
        Task AddMovieToCollection(int movieId, int collectionId);
        Task RemoveMovieFromCollection(int movieId, int collectionId);

        Task<Stream?> GetThumbnail(int movieId);
        Task SetThumbnail(int movieId, Stream thumbnail, BlobContentType contentType);
    }
}
