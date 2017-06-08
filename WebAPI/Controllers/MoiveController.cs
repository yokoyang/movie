using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessEntities;
using BusinessServices;

namespace WebAPI.Controllers
{
    public class MoiveController : ApiController
    {
        private readonly IMovieService _movieService;

        public MoiveController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET api/moive/5
        [Route("api/moive/{moiveId}")]
        public HttpResponseMessage Get(long moiveId)
        {
            var _movie = _movieService.GetMovieById(moiveId);
            if (_movie != null)
                return Request.CreateResponse(HttpStatusCode.OK, _movie);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No movie found for this id");
        }

        //按电影名称查电影
        //       GET http://localhost:51466/api/moives/Blood/1
        [Route("api/moives/{moiveName}/{page}")]
        [HttpGet]
        public HttpResponseMessage MoiveName(string moiveName, int page)
        {
            var _movies = _movieService.GetMovieByName(moiveName, page);


            if (_movies != null)
            {
                var movieEntities = _movies as List<MovieEntity> ?? _movies.ToList();
                if (movieEntities.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _movies);
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No movies found for this id");
        }

        //获得所有的类型
        //     GET   http://localhost:51466/api/alltype
        [Route("api/alltype")]
        [HttpGet]
        public HttpResponseMessage AllType()
        {
            var _typs = _movieService.GetAllType();

            if (_typs != null)
            {
                var movieEntities = _typs as List<GenreEntity> ?? _typs.ToList();
                if (movieEntities.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _typs);
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No type");
        }


        //按电影类别查电影
        [Route("api/type/{moiveType}/{page}")]
        [HttpGet]
        public HttpResponseMessage MoiveType(int moiveType, int page)
        {
            var _movies = _movieService.GetMovieByType(moiveType, page);


            if (_movies != null)
            {
                var movieEntities = _movies as List<MovieEntity> ?? _movies.ToList();
                if (movieEntities.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _movies);
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No movies found for this type");
        }

        //根据名字和类别进行查询
        [Route("api/muti/{moiveType}/{moiveName}/{page}")]
        [HttpGet]
        public HttpResponseMessage MoiveNameType(int moiveType, string moiveName, int page)
        {
            var _movies = _movieService.MoiveNameType(moiveType, moiveName, page);


            if (_movies != null)
            {
                var movieEntities = _movies as List<MovieEntity> ?? _movies.ToList();
                if (movieEntities.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _movies);
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No movies found for this type and name");
        }

    }
}