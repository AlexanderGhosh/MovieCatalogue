using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;
using MovieCatalogue;

namespace MovieApp.Web
{
    public class MovieService(MovieAppContext context, IBlobStorage blobStorage) : IMovieService
    {
        public async Task UploadMovieAsync(IMovie movie, IMovieContent? content)
        {
            if (movie is not Movie m)
                throw new ArgumentException($"Provided {nameof(movie)} must be an instance of {nameof(Movie)}", nameof(movie));
            if (content is not MovieContent and not null)
                throw new ArgumentException($"Provided {nameof(content)} must be an instance of {nameof(MovieContent)}", nameof(content));

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

        public async ValueTask<IMovie?> GetMovie(int movieId)
        {
            return await context.Movies.FindAsync(movieId);
        }

        public async Task<IMovieContent?> GetMovieContent(int movieId)
        {
            var content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is null) return null;
            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
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

        public async Task<IReadOnlyCollection<IMovieCollection>> GetAllCollectionsAsync()
        {
            return await context.Collections.OrderBy(c => c.Name).ToListAsync();
        }

        public async ValueTask<IMovieCollection?> GetCollection(int collectionId)
        {
            return await context.Collections.FindAsync(collectionId);
        }

        public async Task<IReadOnlyCollection<IMovie>> GetMoviesInCollectionAsync(int collectionId)
        {
            return await context.Movies
                .Where(m => m.CollectionId == collectionId)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IMovieCollection> CreateCollection(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var collection = new MovieCollection { Name = name.Trim() };
            context.Collections.Add(collection);
            await context.SaveChangesAsync();
            return collection;
        }

        public async Task RenameCollection(int collectionId, string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var collection = await context.Collections.FindAsync(collectionId);
            if (collection is null) return;
            collection.Name = name.Trim();
            await context.SaveChangesAsync();
        }

        public async Task DeleteCollection(int collectionId)
        {
            var collection = await context.Collections.FindAsync(collectionId);
            if (collection is null) return;

            // Movies are kept; the FK is configured as SET NULL.
            context.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }

        public async Task SetMovieCollection(int movieId, int? collectionId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            if (collectionId is not null && !await context.Collections.AnyAsync(c => c.Id == collectionId))
                return;

            movie.CollectionId = collectionId;
            await context.SaveChangesAsync();
        }

        public async Task<Stream?> GetThumbnail(int movieId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie?.ThumbnailBlobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(movie.ThumbnailBlobId.Value);
            return thumbnail.DataStream;
        }
    }
}
