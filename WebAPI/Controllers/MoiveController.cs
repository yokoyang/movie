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
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No product found for this id");
        }
    }
}