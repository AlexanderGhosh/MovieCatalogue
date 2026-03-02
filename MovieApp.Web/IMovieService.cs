using MovieApp.Web.Models;

namespace MovieApp.Web
{
    public interface IMovieService
    {
        Task UploadMovieAsync(string title, string filePath, string? description = null);
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<string?> GetMovieUrlAsync(int id);
    }
}
