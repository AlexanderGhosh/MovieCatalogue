using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCatalogue
{
    public interface IMovieRepository
    {
        IMovie? GetMovie(int id);
        IEnumerable<IMovie> GetAllMovies();
        void AddMovie(IMovie movie);
    }
}
