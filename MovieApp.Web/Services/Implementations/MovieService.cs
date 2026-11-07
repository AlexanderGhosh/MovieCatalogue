using BlobStorage;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;
using MovieApp.Web.Services.Abstractions;
using MovieApp.Web.Services.Implementations;

namespace MovieApp.Web
{
    public class MovieService(IDbContextFactory<MovieAppContext> contextFactory, IBlobStorage blobStorage) : IMovieService
    {
        private Task<MovieAppContext> GetContext(CancellationToken ct = default)
            => contextFactory.CreateDbContextAsync(ct);

        public async Task UploadMovieAsync(Movie movie, MovieContent? content, CancellationToken ct = default)
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
                await using var context = await GetContext(ct);
                context.Add(movie);
                await context.SaveChangesAsync(ct);
            }
            catch
            {
                await blobStorage.DeleteAsync(contentId);
                throw;
            }
        }

        public async ValueTask<Movie?> GetMovie(int movieId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            return await context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == movieId, ct);
        }

        public async Task<MovieContent?> GetMovieContent(int movieId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var content = await context.MovieContent
                .AsNoTracking()
                .FirstOrDefaultAsync(mc => mc.MovieId == movieId, ct);
            if (content is null) return null;

            var data = await blobStorage.DownloadAsync(content.BlobGuid);
            content.Stream = data.DataStream;
            return content;
        }

        public async Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync(CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            return await context.Movies
                .AsNoTracking()
                .Include(x => x.Collections)
                .ToListAsync(ct);
        }

        public async Task UpdateMovie(int movieId, Action<Movie> update, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(update);

            await using var context = await GetContext(ct);
            var movie = await context.Movies.FindAsync([movieId], ct);
            if (movie is null) return;

            update(movie); // already tracked, so EF only updates changed properties
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteMovie(int movieId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var movie = await context.Movies
                .Include(m => m.Content)
                .FirstOrDefaultAsync(m => m.Id == movieId, ct);
            if (movie is null) return;

            var blobGuid = movie.Content?.BlobGuid;

            if (movie.Content is not null)
                context.MovieContent.Remove(movie.Content);
            context.Movies.Remove(movie);

            await context.SaveChangesAsync(ct);

            // Only delete the blob once the DB change has succeeded
            if (blobGuid is not null)
                await blobStorage.DeleteAsync(blobGuid.Value);
        }

        public async Task<IReadOnlyCollection<MovieCollection>> GetAllCollectionsAsync(CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            return await context.Collections
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Include(x => x.Movies)
                .Include(x => x.ChildCollections)
                .ToListAsync(ct);
        }

        public async ValueTask<MovieCollection?> GetCollection(int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            return await context.Collections
                .AsNoTracking()
                .Include(x => x.Movies)
                .Include(x => x.ChildCollections).ThenInclude(x => x.Movies)
                .Include(x => x.ChildCollections).ThenInclude(x => x.ChildCollections)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == collectionId, ct);
        }

        public async Task<IReadOnlyCollection<Movie>> GetMoviesInCollectionAsync(int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            return await context.Movies
                .AsNoTracking()
                .Where(m => m.Collections.Any(x => x.Id == collectionId))
                .OrderBy(m => m.Title)
                .ToListAsync(ct);
        }

        public async Task<MovieCollection> CreateCollection(string name, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            await using var context = await GetContext(ct);
            var collection = new MovieCollection { Name = name.Trim() };
            context.Collections.Add(collection);
            await context.SaveChangesAsync(ct);
            return collection;
        }

        public async Task RenameCollection(int collectionId, string name, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            await using var context = await GetContext(ct);
            var collection = await context.Collections.FindAsync([collectionId], ct);
            if (collection is null) return;
            collection.Name = name.Trim();
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteCollection(int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var collection = await context.Collections.FindAsync([collectionId], ct);
            if (collection is null) return;

            // Movies are kept; the FK is configured as SET NULL.
            context.Collections.Remove(collection);
            await context.SaveChangesAsync(ct);
        }

        public async Task AddMovieToCollection(int movieId, int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var movie = await context.Movies.FindAsync([movieId], ct);
            if (movie is null) return;

            var collection = await context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId, ct);
            if (collection is null) return;

            movie.Collections.Add(collection);
            await context.SaveChangesAsync(ct);
        }

        public async Task RemoveMovieFromCollection(int movieId, int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var movie = await context.Movies.FindAsync([movieId], ct);
            if (movie is null) return;

            var collection = await context.Collections
                .Include(x => x.Movies)
                .FirstOrDefaultAsync(c => c.Id == collectionId, ct);
            if (collection is null) return;

            collection.Movies.Remove(movie);
            await context.SaveChangesAsync(ct);
        }

        public async Task RemoveCollectionFromCollection(int childId, int parentId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var parent = await context.Collections
                .Include(c => c.ChildCollections)
                .FirstOrDefaultAsync(c => c.Id == parentId, ct);
            if (parent is null) return;

            var child = parent.ChildCollections.FirstOrDefault(c => c.Id == childId);
            if (child is null) return;

            parent.ChildCollections.Remove(child);
            await context.SaveChangesAsync(ct);
        }

        public async Task AddCollectionToCollection(int parentId, int childId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var parent = await context.Collections
                .Include(c => c.ChildCollections)
                .FirstOrDefaultAsync(c => c.Id == parentId, ct);
            var child = await context.Collections.FindAsync([childId], ct);
            if (parent is null || child is null) return;

            if (parent.ChildCollections.Any(c => c.Id == childId)) return; // already a child

            parent.ChildCollections.Add(child);
            await context.SaveChangesAsync(ct);
        }

        public async Task<Thumbnail?> GetMovieThumbnail(int movieId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var blobId = await context.Movies
                .AsNoTracking()
                .Where(m => m.Id == movieId)
                .Select(m => m.ThumbnailBlobId)
                .FirstOrDefaultAsync(ct);
            if (blobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(blobId.Value);
            return thumbnail.DataStream is null ? null : new(thumbnail.DataStream, thumbnail.MetaData.ContentType);
        }

        public async Task SetMovieThumbnail(int movieId, Thumbnail thumbnail, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var movie = await context.Movies.FindAsync([movieId], ct);
            if (movie is null) return;

            var oldBlobId = movie.ThumbnailBlobId;
            var newBlobId = await blobStorage.UploadAsync(thumbnail.Stream, new() { ContentType = thumbnail.ContentType });
            movie.ThumbnailBlobId = newBlobId;

            try
            {
                await context.SaveChangesAsync(ct);
            }
            catch
            {
                await blobStorage.DeleteAsync(newBlobId);
                throw;
            }

            if (oldBlobId is not null)
                await blobStorage.DeleteAsync(oldBlobId.Value);
        }

        public async Task<Thumbnail?> GetCollectionThumbnail(int collectionId, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var blobId = await context.Collections
                .AsNoTracking()
                .Where(c => c.Id == collectionId)
                .Select(c => c.ThumbnailBlobId)
                .FirstOrDefaultAsync(ct);
            if (blobId is null) return null;

            var thumbnail = await blobStorage.DownloadAsync(blobId.Value);
            return thumbnail.DataStream is null ? null : new(thumbnail.DataStream, thumbnail.MetaData.ContentType);
        }

        public async Task SetCollectionThumbnail(int collectionId, Thumbnail thumbnail, CancellationToken ct = default)
        {
            await using var context = await GetContext(ct);
            var collection = await context.Collections.FindAsync([collectionId], ct);
            if (collection is null) return;

            var oldBlobId = collection.ThumbnailBlobId;
            var newBlobId = await blobStorage.UploadAsync(thumbnail.Stream, new() { ContentType = thumbnail.ContentType });
            collection.ThumbnailBlobId = newBlobId;

            try
            {
                await context.SaveChangesAsync(ct);
            }
            catch
            {
                await blobStorage.DeleteAsync(newBlobId);
                throw;
            }

            if (oldBlobId is not null)
                await blobStorage.DeleteAsync(oldBlobId.Value);
        }
    }
}
