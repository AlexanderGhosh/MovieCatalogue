using MovieCatalogue;

namespace MovieApp.Web
{
    public interface IMovieService
    {
        Task UploadMovieAsync(IMovie movie, IMovieContent? content);
        ValueTask<IMovie?> GetMovie(int movieId);
        Task<IMovieContent?> GetMovieContent(int movieId);
        Task<IReadOnlyCollection<IMovie>> GetAllMoviesAsync();

        Task UpdateMovie<TMovie>(int movieId, Action<TMovie> update)
            where TMovie : class, IMovie;

        Task DeleteMovie(int movieId);

        Task<IReadOnlyCollection<IMovieCollection>> GetAllCollectionsAsync();
        ValueTask<IMovieCollection?> GetCollection(int collectionId);
        Task<IReadOnlyCollection<IMovie>> GetMoviesInCollectionAsync(int collectionId);
        Task<IMovieCollection> CreateCollection(string name);
        Task RenameCollection(int collectionId, string name);
        Task DeleteCollection(int collectionId);
        Task SetMovieCollection(int movieId, int? collectionId);

        Task<Stream?> GetThumbnail(int movieId);
    }
}
