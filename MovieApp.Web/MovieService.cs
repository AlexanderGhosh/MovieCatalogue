using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;
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
            if (content is not MovieContent and not null)
                throw new ArgumentException($"Provided content must be an instance of {nameof(MovieContent)}", nameof(content));
            if (thumbnail is not Thumbnail and not null)
                throw new ArgumentException($"Provided thumbnail must be an instance of {nameof(Thumbnail)}", nameof(thumbnail));

            if (content is not null)
            {
                MovieContent mc = (MovieContent)content;
                using var movieStream = content.Stream;
                Guid contentId = await blobStorage.UploadAsync(movieStream, $"Movie Content: {movie.Title}");
                mc.BlobGuid = contentId;
                m.Content = mc;
            }

            context.Add(m);
            await context.SaveChangesAsync();
        }

        public async Task<IMovieContent?> GetMovieContent(int movieId)
        {
            MovieContent? content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is null) return null;
            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
        }

        public Task<IThumbnail?> GetMovieThumbnail(int movieId)
        {
            throw new NotImplementedException();
        }
    }
}
