using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;
using MovieCatalogue;
using MovieCatalogue.Models;

namespace MovieApp.Web
{
    public class MovieService(MovieAppContext context, IBlobStorage blobStorage) : IMovieService
    {
        public async Task UploadMovieAsync(IMovie movie, IMovieContent? content, IThumbnail? thumbnail)
        {
            if (movie is not Movie m)
                throw new ArgumentException($"Provided {nameof(movie)} must be an instance of {nameof(Movie)}", nameof(movie));
            if (content is not MovieContent and not null)
                throw new ArgumentException($"Provided {nameof(content)} must be an instance of {nameof(MovieContent)}", nameof(content));
            if (thumbnail is not Thumbnail and not null)
                throw new ArgumentException($"Provided {nameof(thumbnail)} must be an instance of {nameof(Thumbnail)}", nameof(thumbnail));

            if (content is not null)
            {
                var mc = (MovieContent)content;
                using var movieStream = content.Stream;
                var metaData = new BlobMetaData()
                {
                    ContentType = BlobContentTypes.MP4
                };
                var contentId = await blobStorage.UploadAsync(movieStream, metaData);
                mc.BlobGuid = contentId;
                m.Content = content as MovieContent;
            }

            context.Add(m);
            await context.SaveChangesAsync();
        }

        public async Task<IMovieContent?> GetMovieContent(int movieId)
        {
            var content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is null) return null;
            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
        }

        public Task<IThumbnail?> GetMovieThumbnail(int movieId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<IMovie>> GetAllMoviesAsync()
        {
            return await context.Movies.ToListAsync();
        }

        public async Task UpdateMovie<TMovie>(int movieId, Action<TMovie> update)
            where TMovie : class, IMovie
        {
            ArgumentNullException.ThrowIfNull(update);

            var movie = await context.Set<TMovie>().FindAsync(movieId);
            if (movie is null) return;
            update(movie);
            context.Set<TMovie>().Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMovie(int movieId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            var content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is not null)
            {
                context.MovieContent.Remove(content);
                await blobStorage.DeleteAsync(content.BlobGuid);
            }
            context.Movies.Remove(movie);
            await context.SaveChangesAsync();
        }
    }
}
