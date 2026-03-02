using System.Collections.Concurrent;

namespace MovieCatalogue
{
    public class MovieRepository : IMovieRepository
    {
        protected readonly ConcurrentDictionary<int, IMovie> _movies = [];
        public virtual void AddMovie(IMovie movie)
        {
            ArgumentNullException.ThrowIfNull(movie, nameof(movie));
            if (_movies.ContainsKey(movie.Id))
                throw new ArgumentException($"A movie with Id {movie.Id} already exists.");
            _movies.TryAdd(movie.Id, movie);
        }

        public virtual IEnumerable<IMovie> GetAllMovies() => _movies.Values;

        public virtual IMovie? GetMovie(int id)
        {
            _movies.TryGetValue(id, out var movie);
            return movie;
        }
    }
}
