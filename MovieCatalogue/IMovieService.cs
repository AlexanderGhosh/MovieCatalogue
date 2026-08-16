using MovieCatalogue;

namespace MovieApp.Web
{
    public interface IMovieService
    {
        Task UploadMovieAsync(IMovie movie, IMovieContent? content, IThumbnail? thumbnail);
        Task<IMovieContent?> GetMovieContent(int movieId);
        Task<IThumbnail?> GetMovieThumbnail(int movieId);
        Task<IReadOnlyCollection<IMovie>> GetAllMoviesAsync();

        Task UpdateMovie<TMovie>(int movieId, Action<TMovie> update)
            where TMovie : class, IMovie;

        Task DeleteMovie(int movieId);
    }
}
