using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    public class MovieService : IMovieService
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public MovieService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<movie, MovieEntity>();
                cfg.CreateMap<net_user, UserEntity>();
            });
        }

        /// <summary>
        /// Fetches Movie details by id
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public MovieEntity GetMovieById(long movieId)
        {
            var _moive = _unitOfWork.MovieRepository.GetByID(movieId);

            if (_moive != null)
            {
                var movieModel = Mapper.Map<movie, MovieEntity>(_moive);
                return movieModel;
            }

            return null;
        }

        /// <summary>
        /// Fetches all the Movies.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.MovieEntity> GetAllMovies()
        {
            var _moives = _unitOfWork.MovieRepository.GetAll().ToList();
            if (_moives.Any())
            {
                var moviesModel = Mapper.Map<List<movie>, List<MovieEntity>>(_moives);
                return moviesModel;
            }
            return null;
        }

        public long CreateMovie(MovieEntity movieEntity)
        {
            var _moive = new movie()
            {
                asin = movieEntity.asin,
                score = movieEntity.score,
                time_id = movieEntity.time_id,
                title = movieEntity.title,
                movie_money = movieEntity.movie_money
            };
            _unitOfWork.MovieRepository.Insert(_moive);
            _unitOfWork.Save();
            return _moive.movie_id;
        }


        public bool UpdateMovie(long movieId, MovieEntity movieEntity)
        {
            var success = false;
            if (movieEntity != null)
            {
                var _moive = _unitOfWork.MovieRepository.GetByID(movieId);
                if (_moive != null)
                {
                    _moive.asin = movieEntity.asin;
                    _moive.score = movieEntity.score;
                    _moive.time_id = movieEntity.time_id;
                    _moive.title = movieEntity.title;
                    _moive.movie_money = movieEntity.movie_money;

                    _unitOfWork.MovieRepository.Update(_moive);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        public bool DeleteMovie(long movieId)
        {
            var success = false;
            if (movieId > 0)
            {
                var _moive = _unitOfWork.MovieRepository.GetByID(movieId);
                if (_moive != null)
                {
                    _unitOfWork.MovieRepository.Delete(_moive);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        private string GetPicUrl(int counter)
        {
            counter = counter % 400;
            var rnd = new Random();
            var id = rnd.Next(600);
            id += counter;
            var _pic = _unitOfWork.PictureRepository.GetByID(id);
            return _pic.picture_url;
        }

        public List<MovieEntity> GetMovieByName(string movieName, int page)
        {
            string startPage = ((page - 1) * 20).ToString();

            String sql = "SELECT * FROM datawarehouse.movie where title like '%" + movieName + "%' limit " +
                         startPage + ",20";
            var context = new WebApiDbEntities();

            var _movies = context.Database.SqlQuery<movie>(sql).ToList();
            if (_movies.Any())
            {
                var moviesModel = Mapper.Map<List<movie>, List<MovieEntity>>(_movies);
                int counter = 1;
                foreach (var movieModel in moviesModel)
                {
                    counter++;
                    movieModel.pic_url = GetPicUrl(counter);
                }
                return moviesModel;
            }
            return null;

            //            var _movies = _unitOfWork.MovieRepository.GetMany(m => m.title == movieName).ToList();
            //            if (_movies.Any())
            //            {
            //                var moviesModel = Mapper.Map<List<movie>, List<MovieEntity>>(_movies);
            //                return moviesModel;
            //            }
            //            return null;
        }

        public List<MovieEntity> GetMovieByType(int moiveType, int page)
        {
            string startPage = ((page - 1) * 20).ToString();
            string sql = "SELECT movie_id FROM datawarehouse.movie_genre where genre_id ='" + moiveType + "' limit " +
                         startPage + ",20";

            var context = new WebApiDbEntities();
            var _moviesIds = context.Database.SqlQuery<long>(sql).ToList();
            List<movie> _movies = new List<movie>();
            foreach (var movieId in _moviesIds)
            {
                _movies.Add(_unitOfWork.MovieRepository.GetByID(movieId));
            }


            if (_movies.Any())
            {
                var moviesModel = Mapper.Map<List<movie>, List<MovieEntity>>(_movies);
                int counter = 1;

                foreach (var movieModel in moviesModel)
                {
                    counter++;
                    movieModel.pic_url = GetPicUrl(counter);
                }
                return moviesModel;
            }
            return null;
        }

        public List<GenreEntity> GetAllType()
        {
            var _type = _unitOfWork.GenreRepository.GetAll().ToList();
            if (_type.Any())
            {
                var genreModel = Mapper.Map<List<genre>, List<GenreEntity>>(_type);
                return genreModel;
            }
            return null;
        }

        public List<MovieEntity> MoiveNameType(int moiveType, string moiveName, int page)
        {
            string startPage = ((page - 1) * 20).ToString();
            string sql =
                "SELECT * FROM datawarehouse.movie_genre m_g join datawarehouse.movie m on m_g.movie_id=m.movie_id where m_g.genre_id ='"
                + moiveType + "' and m.title like '%" + moiveName + "%' limit " + startPage + ",20";
            var context = new WebApiDbEntities();


            var _movies = context.Database.SqlQuery<movie>(sql).ToList();
            if (_movies.Any())
            {
                var moviesModel = Mapper.Map<List<movie>, List<MovieEntity>>(_movies);
                int counter = 1;
                foreach (var movieModel in moviesModel)
                {
                    counter++;
                    movieModel.pic_url = GetPicUrl(counter);
                }
                return moviesModel;
            }
            return null;
        }
    }
}