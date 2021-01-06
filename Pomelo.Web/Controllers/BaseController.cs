using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pomelo.Web.Controllers
{
    /// <summary>
    /// Base控制器
    /// </summary>
    public class BaseController : ControllerBase
    {

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogger<BaseController> _Logger; 

        protected BaseController(ILogger<BaseController> logger)
        {
            _Logger = logger; 

        }



        /// <summary>
        /// 获取后台登陆用户的用户id
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetAdminUserId()
        {
            try
            {
                return User.Identity.Name;
            }
            catch
            {
                return "";
            }
        }



        /// <summary>
        /// 获取客户端ip
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetClientIP()
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (ip == "127.0.0.1")
            {
                //nginx 代理情况下获取ip
                ip = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            return ip;
        }
         

    }
}