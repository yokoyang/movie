using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;

namespace BusinessServices
{
    public interface IMovieService
    {
        MovieEntity GetMovieById(long movieId);
        IEnumerable<MovieEntity> GetAllMovies();
        long CreateMovie(MovieEntity movieEntity);
        bool UpdateMovie(long movieId, MovieEntity movieEntity);
        bool DeleteMovie(long movieId);
    }
}
