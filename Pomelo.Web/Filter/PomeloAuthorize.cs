using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json; 
using Pomelo.Web.Controllers.File.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Filter
{
   
    public class PomeloAuthorize : ActionFilterAttribute
    {  
        public override void OnActionExecuting(ActionExecutingContext context)
        { 
           
            string appId = context.HttpContext.Request.Query["AppId"];
            string appSecret = context.HttpContext.Request.Query["AppSecret"];
 
            if (!CheckUser(appId, appSecret))
            {
                ErrorReturn(context, "身份异常");
                return;
            }  
        }


        private void ErrorReturn(ActionExecutingContext filterContext, string message)
        {

            //异常统一输出 
            filterContext.HttpContext.Response.ContentType = "application/json;charset=utf-8";
            filterContext.HttpContext.Response.StatusCode = 401;
            filterContext.Result = new Microsoft.AspNetCore.Mvc.JsonResult(new
            {
                code = 1,
                data = "",
                message = message,
                time = DateTime.Now.ToString()
            });
        }


        private bool CheckUser(string appId, string appSecret)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var auth = config.GetSection("Auth").Get<AuthModel>();

            if (!auth.IsCheck)
            {
                return true;
            }
            else
            {
                if (appId == auth.AppId && appSecret == auth.AppSecret)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
