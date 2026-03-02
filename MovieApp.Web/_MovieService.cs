using BlobStorage;
using MovieApp.Web;
using Microsoft.EntityFrameworkCore;

namespace MovieCatalogue
{
    public class _MovieService
    {
        //private readonly DbContext _db;
        //private readonly IBlobStorage _blob;
        //public MovieService(DbContext db, IBlobStorage blob)
        //{
        //    _db = db;
        //    _blob = blob;
        //}

        //public Task<IEnumerable<IMovie>> GetAllMoviesAsync()
        //{
        //    return Task.FromResult(_db.Set<IMovie>().AsEnumerable());
        //}

        ////public async Task<string?> GetMovieUrlAsync(int id)
        ////{
        ////    Movie? movie = await _db.Set<Movie>().FindAsync(id);
        ////    if (movie is null || movie.MovieGuid is null) return null;
        ////    if (!(await _blob.ExistsAsync(movie.MovieGuid.Value)))
        ////        return null;

        ////    string url = _blob.GetBlobFilePath(movie.MovieGuid.Value);
        ////    string tmpPath = Path.ChangeExtension(url, ".mp4");
        ////    File.Copy(url, tmpPath, true);
        ////    return tmpPath;
        ////}

        //public async Task UploadMovieAsync(string title, string filePath, string? description = null) 
        //{
        //    ArgumentNullException.ThrowIfNullOrEmpty(title, nameof(title));
        //    ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

        //    if (!File.Exists(filePath))
        //        throw new FileNotFoundException("The provided file path does not exist.", filePath);

        //    Guid blobId;
        //    using (var stream = File.OpenRead(filePath))
        //    {
        //        blobId = await _blob.UploadAsync(stream, Path.GetFileName(filePath));
        //    }
        //    IMovie movie = new()
        //    {
        //        Title = title,
        //        Length = TimeSpan.FromSeconds(0),
        //        MovieGuid = blobId
        //    };
        //    _db.Set<IMovie>().Add(movie);
        //    await _db.SaveChangesAsync();
        //}
    }
}
