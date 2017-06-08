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
    public class AdminController : ApiController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        //登录
        //post [name:xxx,password:xxx]
        //返回形式:
        //        {
        //        "name": "luge",
        //        "result": "0"
        //    }
        [Route("api/admin/login")]
        [HttpPost]
        public JsonResult<Dictionary<string, string>> BuyMovie([FromBody] JObject jdata)
        {
            int result = -1;
            var info = new Dictionary<string, string>();
            dynamic json = jdata;
            string name = json.name;
            string password = json.password;

            result = _adminService.AdminLogin(name, password);
            //登录成功
            if (result == 0)
            {
                info.Add("name", name);
            }
            info.Add("result", result.ToString());
            return Json(info);
        }
    }
}