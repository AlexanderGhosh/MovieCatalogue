using BlobStorage;
using MovieCatalogue;

namespace MovieApp.Web
{
    public class MovieService(MovieAppContext context, IBlobStorage blobStorage) : IMovieService
    {
        public Task<IEnumerable<IMovie>> GetAllMoviesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IMovieContent?> GetMovieContent(int movieId)
        {
            throw new NotImplementedException();
        }

        public Task<IThumbnail?> GetMovieThumbnail(int movieId)
        {
            throw new NotImplementedException();
        }

        public Task UploadMovieAsync(IMovie movie, IMovieContent? content, IThumbnail? thumbnail)
        {
            throw new NotImplementedException();
        }
    }
}
