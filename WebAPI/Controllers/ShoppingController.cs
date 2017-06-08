using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using BusinessServices;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    public class ShoppingController : ApiController
    {
        private readonly IShopCartService _shopCartService;

        public ShoppingController(IShopCartService shopCartService)
        {
            _shopCartService = shopCartService;
        }

        //查看已购买商品
        [Route("api/shopList")]
        [Authorize]
        public HttpResponseMessage Get()
        {
            //如果返回的是401 表示是未授权登录
            int state = 0;
            var result = _shopCartService.QueryChart(User.Identity.Name, state);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

//        http://localhost:51466/api/history
//查看删除掉的商品
        [Route("api/History")]
        [Authorize]
        public HttpResponseMessage GetHistory()
        {
            //如果返回的是401 表示是未授权登录
            int state = 2;
            var result = _shopCartService.QueryChart(User.Identity.Name, state);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        // 查看购物车trolley中的商品
        [Route("api/trolley")]
        [Authorize]
        public HttpResponseMessage GetTrolley()
        {
            //如果返回的是401 表示是未授权登录
            int state = 1;
            var result = _shopCartService.QueryChart(User.Identity.Name, state);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        //直接购买
        [Route("api/buy")]
        [Authorize]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> BuyMovie([FromBody] JObject jdata)
        {
            var info = new Dictionary<string, string>();
            dynamic json = jdata;
            int movieId = json.movieId;
            int amount = json.amount;
            int result = -1;
            if (amount > 0)
            {
                result = _shopCartService.BuyMovie(movieId, amount, User.Identity.Name);
            }
            info.Add("result", result.ToString());
            return Json(info);
        }

        //添加到购物车
        [Route("api/add")]
        [Authorize]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> AddMovie([FromBody] JObject jdata)
        {
            var info = new Dictionary<string, string>();
            dynamic json = jdata;

            int result = -1;
            int movieId = json.movieId;
            int amount = json.amount;
            result = _shopCartService.AddMovie(movieId, amount, User.Identity.Name);
            info.Add("result", result.ToString());
            return Json(info);
        }


        //购物车中删除商品
        [Route("api/delete/{movieId}")]
        [Authorize]
        [HttpDelete]
        public JsonResult<Dictionary<string, string>> DeleteMovie(int movieId)
        {
            var info = new Dictionary<string, string>();

            int result = -1;

            result = _shopCartService.DeleteChartMovie(movieId, User.Identity.Name);
            info.Add("result", result.ToString());
            return Json(info);
        }
    }
}