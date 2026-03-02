using BlobStorage;
using MovieApp.Web.Models;

namespace MovieApp.Web
{
    public class MovieService : IMovieService
    {
        private readonly MovieAppContext _db;
        private readonly LocalBlobStorage _blob;
        public MovieService(MovieAppContext db, LocalBlobStorage blob)
        {
            _db = db;
            _blob = blob;
        }

        public Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return Task.FromResult(_db.Movies.AsEnumerable());
        }

        public async Task<string?> GetMovieUrlAsync(int id)
        {
            Movie? movie = await _db.Movies.FindAsync(id);
            if (movie is null || movie.MovieGuid is null) return null;
            if (!(await _blob.ExistsAsync(movie.MovieGuid.Value)))
                return null;

            string url = _blob.GetBlobFilePath(movie.MovieGuid.Value);
            string tmpPath = Path.ChangeExtension(url, ".mp4");
            File.Copy(url, tmpPath, true);
            return tmpPath;
        }

        public async Task UploadMovieAsync(string title, string filePath, string? description = null) 
        {
            ArgumentNullException.ThrowIfNullOrEmpty(title, nameof(title));
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The provided file path does not exist.", filePath);

            Guid blobId;
            using (var stream = File.OpenRead(filePath))
            {
                blobId = await _blob.UploadAsync(stream, Path.GetFileName(filePath));
            }
            Movie movie = new()
            {
                Title = title,
                Length = TimeSpan.FromSeconds(0),
                MovieGuid = blobId
            };
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
        }
    }
}
