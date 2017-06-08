using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using BusinessEntities;
using BusinessServices;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    public class ConcessionController : ApiController
    {
        //依赖注入
        private IConcessionService _concessionService;

        public ConcessionController(IConcessionService concessionService)
        {
            _concessionService = concessionService;
        }

        [Route("api/join")]
        [Authorize]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> JoinConcession([FromBody] JObject jdata)
        {
            _concessionService.UpdateConcessionRecord();
            var info = new Dictionary<string, string>();
            dynamic json = jdata;
            int concessionId = json.concessionId;
            int result = -1;
            result = _concessionService.JoinGame(concessionId, User.Identity.Name);
            info.Add("result", result.ToString());
            return Json(info);
        }


        //管理员发布抽奖活动
        //[movieId, concessionName, startTime, endTime, price, amount]
        [Route("api/publish")]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> PublishConcession(PublishConcessionEntity publishConcession)
        {
            _concessionService.UpdateConcessionRecord();
            int result = -1;
            var info = new Dictionary<string, string>();

            result = _concessionService.PublishConcession(publishConcession);


            info.Add("result", result.ToString());
            return Json(info);
        }

        //查看抽奖结果 ReviewResult
        [Route("api/review/{state}")]
        [Authorize]
        [HttpGet]
        public List<ConcessionResultInfo> ReviewResult(int state)
        {
            _concessionService.UpdateConcessionRecord();
            List<ConcessionResultInfo> concessionResultInfos = new List<ConcessionResultInfo>();
            concessionResultInfos = _concessionService.ReviewResult(User.Identity.Name, state);
            return concessionResultInfos;
        }


        // 显示所有可用的抽奖活动 ShowTheConcessions
        [Route("api/showme")]
        [HttpGet]
        public List<ConcessionEntity> ShowMeConcession()
        {
            _concessionService.UpdateConcessionRecord();
            List<ConcessionEntity> concessionEntities = new List<ConcessionEntity>();
            concessionEntities = _concessionService.ShowTheConcessions();
            return concessionEntities;
        }

        //根据Id得到具体信息 GetConsessionById

        [Route("api/detail/{concessionId}")]
        [HttpGet]
        public ConcessionEntity Detail(int concessionId)
        {
            _concessionService.UpdateConcessionRecord();
            return _concessionService.GetConsessionById(concessionId);
        }


        //支付抽奖
        //[concession: xxx]
        [Route("api/luckyPay")]
        [Authorize]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> LuckyPay([FromBody] JObject jdata)
        {
            dynamic json = jdata;

            int concessionId = json.concession;
            _concessionService.UpdateConcessionRecord();
            int result = -1;
            var info = new Dictionary<string, string>();
            result = _concessionService.PayConcession(User.Identity.Name, concessionId);

            info.Add("result", result.ToString());
            return Json(info);
        }


        //更新
        [Route("api/update")]
        [Authorize]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> Update()
        {
            int result = -1;
            var info = new Dictionary<string, string>();
            _concessionService.UpdateConcessionRecord();

            info.Add("result", result.ToString());
            return Json(info);
        }
    }
}