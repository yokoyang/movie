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
        List<MovieEntity> GetMovieByName(string movieName, int page);
        List<MovieEntity> GetMovieByType(int moiveType, int page);

        //得到所有的类型
        List<GenreEntity> GetAllType();

        //名字+类型
        List<MovieEntity> MoiveNameType(int moiveType, string moiveName, int page);
    }
}