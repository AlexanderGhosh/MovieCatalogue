using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;
using MovieApp.Web.Services.Abstractions;
using MovieApp.Web.Services.Implementations;

namespace MovieApp.Web
{
    public class MovieService(IDbContextFactory<MovieAppContext> contextFactory, IBlobStorage blobStorage) : IMovieService
    {
        private Task<MovieAppContext> GetContext(CancellationToken cancellationToken = default)
        {
            return contextFactory.CreateDbContextAsync(cancellationToken);
        }

        public async Task UploadMovieAsync(Movie movie, MovieContent? content)
        {
            ArgumentNullException.ThrowIfNull(movie);
            ArgumentNullException.ThrowIfNull(content);

            using var movieStream = content.Stream;
            var metaData = new BlobMetaData { ContentType = BlobContentTypes.MP4 };
            var contentId = await blobStorage.UploadAsync(movieStream, metaData);
            content.BlobGuid = contentId;
            movie.Content = content;

            try
            {
                await using var context = await GetContext();
                context.Add(movie);
                await context.SaveChangesAsync();
            }
            catch
            {
                await blobStorage.DeleteAsync(contentId);
                throw;
            }
        }

        public async ValueTask<Movie?> GetMovie(int movieId)
        {
            await using var context = await GetContext();
            return await context.Movies.FindAsync(movieId);
        }

        public async Task<MovieContent?> GetMovieContent(int movieId)
        {
            await using var context = await GetContext();
            var content = await context.MovieContent.FirstOrDefaultAsync(mc => mc.MovieId == movieId);
            if (content is null) return null;
            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
        }
        public async Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync()
        {
            await using var context = await GetContext();
            return await context.Movies
                .Include(x => x.Collections)
                .ToListAsync();
        }

        public async Task UpdateMovie(int movieId, Action<Movie> update)
        {
            ArgumentNullException.ThrowIfNull(update);

            await using var context = await GetContext();
            var movie = await context.Set<Movie>().FindAsync(movieId);
            if (movie is null) return;
            update(movie);
            context.Set<Movie>().Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMovie(int movieId)
        {
            await using var context = await GetContext();
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
            await using var context = await GetContext();
            return await context.Collections.OrderBy(c => c.Name)
                .Include(x => x.Movies)
                .Include(x => x.ChildCollections)
                .ToListAsync();
        }

        public async ValueTask<MovieCollection?> GetCollection(int collectionId)
        {
            await using var context = await GetContext();
            return await context.Collections
                .Include(x => x.Movies)
                .Include(x => x.ChildCollections)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == collectionId);
        }

        public async Task<IReadOnlyCollection<Movie>> GetMoviesInCollectionAsync(int collectionId)
        {
            await using var context = await GetContext();
            return await context.Movies
                .Where(m => m.Collections.Any(x => x.Id == collectionId))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<MovieCollection> CreateCollection(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            await using var context = await GetContext();
            var collection = new MovieCollection { Name = name.Trim() };
            context.Collections.Add(collection);
            await context.SaveChangesAsync();
            return collection;
        }

        public async Task RenameCollection(int collectionId, string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            await using var context = await GetContext();
            var collection = await context.Collections.FindAsync(collectionId);
            if (collection is null) return;
            collection.Name = name.Trim();
            await context.SaveChangesAsync();
        }

        public async Task DeleteCollection(int collectionId)
        {
            await using var context = await GetContext();
            var collection = await context.Collections.FindAsync(collectionId);
            if (collection is null) return;

            // Movies are kept; the FK is configured as SET NULL.
            context.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }

        public async Task AddMovieToCollection(int movieId, int collectionId)
        {
            await using var context = await GetContext();
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
            await using var context = await GetContext();
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            var collection = await context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
            if (collection is null)
                return;

            movie.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }

        public async Task RemoveCollectionFromCollection(int childId, int parentId)
        {
            await using var context = await GetContext();
            var parent = await context.Collections.FindAsync(parentId);
            var child = await context.Collections.FindAsync(childId);
            if (parent is null || child is null) return;

            parent.ChildCollections.Remove(child);
            await context.SaveChangesAsync();
        }

        public async Task AddCollectionToCollection(int parentId, int childId)
        {
            await using var context = await GetContext();
            var parent = await context.Collections.FindAsync(parentId);
            var child = await context.Collections.FindAsync(childId);
            if (parent is null || child is null) return;
            parent.ChildCollections.Add(child);
            await context.SaveChangesAsync();
        }

        public async Task<Thumbnail?> GetMovieThumbnail(int movieId)
        {
            await using var context = await GetContext();
            var movie = await context.Movies.FindAsync(movieId);
            if (movie?.ThumbnailBlobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(movie.ThumbnailBlobId.Value);
            return thumbnail.DataStream is null ? null : new(thumbnail.DataStream, thumbnail.MetaData.ContentType);
        }

        public async Task SetMovieThumbnail(int movieId, Thumbnail thumbnail)
        {
            await using var context = await GetContext();
            var movie = await context.Movies.FindAsync(movieId);
            if (movie is null) return;

            var oldBlobId = movie.ThumbnailBlobId;
            var newBlobId = await blobStorage.UploadAsync(thumbnail.Stream, new() { ContentType = thumbnail.ContentType });
            movie.ThumbnailBlobId = newBlobId;

            try
            {
                await context.SaveChangesAsync();
            }
            catch
            {
                await blobStorage.DeleteAsync(newBlobId);
                throw;
            }

            if (oldBlobId is not null)
                await blobStorage.DeleteAsync(oldBlobId.Value);
        }

        public async Task<Thumbnail?> GetCollectionThumbnail(int collectionId)
        {
            await using var context = await GetContext();
            var collection = await context.Collections.FindAsync(collectionId);
            if (collection?.ThumbnailBlobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(collection.ThumbnailBlobId.Value);
            return thumbnail.DataStream is null ? null : new(thumbnail.DataStream, thumbnail.MetaData.ContentType);
        }

        public async Task SetCollectionThumbnail(int collectionId, Thumbnail thumbnail)
        {
            await using var context = await GetContext();
            var collection = await context.Collections.FindAsync(collectionId);
            if (collection is null) return;
            if (collection.ThumbnailBlobId is not null)
                await blobStorage.DeleteAsync(collection.ThumbnailBlobId.Value);
            var id = await blobStorage.UploadAsync(thumbnail.Stream, new() { ContentType = thumbnail.ContentType });
            collection.ThumbnailBlobId = id;
            await context.SaveChangesAsync();
        }
    }
}
