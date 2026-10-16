using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;

namespace MovieApp.Web
{
    public class MovieService(MovieAppContext context, IBlobStorage blobStorage) : IMovieService
    {
        public async Task UploadMovieAsync(Movie movie, MovieContent? content)
        {
            ArgumentNullException.ThrowIfNull(movie);
            ArgumentNullException.ThrowIfNull(content);

            if (content is not null)
            {
                using var movieStream = content.Stream;
                var metaData = new BlobMetaData()
                {
                    ContentType = BlobContentTypes.MP4
                };
                var contentId = await blobStorage.UploadAsync(movieStream, metaData);
                content.BlobGuid = contentId;
                movie.Content = content;
            }

            context.Add(movie);
            await context.SaveChangesAsync();
        }

        public async ValueTask<Movie?> GetMovie(int movieId)
        {
            return await context.Movies.FindAsync(movieId);
        }

        public async Task<MovieContent?> GetMovieContent(int movieId)
        {
            var content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is null) return null;
            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
        }

        public async Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync()
        {
            return await context.Movies.ToListAsync();
        }

        public async Task UpdateMovie(int movieId, Action<Movie> update)
        {
            ArgumentNullException.ThrowIfNull(update);

            var movie = await context.Set<Movie>().FindAsync(movieId);
            if (movie is null) return;
            update(movie);
            context.Set<Movie>().Update(movie);
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

        public async Task<IReadOnlyCollection<MovieCollection>> GetAllCollectionsAsync()
        {
            return await context.Collections.OrderBy(c => c.Name)
                .Include(x => x.Movies)
                .Include(x => x.ChildCollections)
                .ToListAsync();
        }

        public async ValueTask<MovieCollection?> GetCollection(int collectionId)
        {
            return await context.Collections.FindAsync(collectionId);
        }

        public async Task<IReadOnlyCollection<Movie>> GetMoviesInCollectionAsync(int collectionId)
        {
            return await context.Movies
                .Where(m => m.Collections.Any(x => x.Id == collectionId))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<MovieCollection> CreateCollection(string name)
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

        public async Task AddMovieToCollection(int movieId, int collectionId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            var collection = await context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
            if (collection is null)
                return;

            movie.Collections.Add(collection);
            await context.SaveChangesAsync();
        }

        public async Task RemoveMovieFromCollection(int movieId, int collectionId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            var collection = await context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
            if (collection is null)
                return;

            movie.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }

        public async Task<Stream?> GetThumbnail(int movieId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie?.ThumbnailBlobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(movie.ThumbnailBlobId.Value);
            return thumbnail.DataStream;
        }

        public async Task SetThumbnail(int movieId, Stream thumbnail, BlobContentType contentType)
        {
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;
            if (movie.ThumbnailBlobId is not null)
                await blobStorage.DeleteAsync(movie.ThumbnailBlobId.Value);
            var id = await blobStorage.UploadAsync(thumbnail, new() { ContentType = contentType });
            movie.ThumbnailBlobId = id;
            await context.SaveChangesAsync();
        }
    }
}
