using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieCatalogue;
using MovieCatalogue.Models;

namespace MovieApp.Web
{
    public class MovieService(MovieAppContext context, IBlobStorage blobStorage) : IMovieService
    {
        public async Task<IReadOnlyCollection<IMovie>> GetAllMoviesAsync()
        {
            return await context.Movies.ToListAsync();
        }

        public async Task UploadMovieAsync(IMovie movie, IMovieContent? content, IThumbnail? thumbnail)
        {
            if (movie is not Movie m) 
                throw new ArgumentException($"Provided movie must be an instance of {nameof(Movie)}", nameof(movie));

            context.Add(m);

            await context.SaveChangesAsync();
        }

        public Task<IMovieContent?> GetMovieContent(int movieId)
        {
            throw new NotImplementedException();
        }

        public Task<IThumbnail?> GetMovieThumbnail(int movieId)
        {
            throw new NotImplementedException();
        }
    }
}
