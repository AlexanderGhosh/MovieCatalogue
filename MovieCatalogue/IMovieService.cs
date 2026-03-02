using MovieCatalogue;

namespace MovieApp.Web
{
    public interface IMovieService
    {
        Task UploadMovieAsync(IMovie movie, IMovieContent? content, IThumbnail? thumbnail);
        Task<IMovieContent?> GetMovieContent(int movieId);
        Task<IThumbnail?> GetMovieThumbnail(int movieId);
        Task<IEnumerable<IMovie>> GetAllMoviesAsync();
    }
}
