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
            //            _unitOfWork = new UnitOfWork();

            Mapper.Initialize(cfg => cfg.CreateMap<movie, MovieEntity>());

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
    }
}