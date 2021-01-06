using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Pomelo.Interface;
using Pomelo.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Filter
{
   
    public class PomeloAuthorize : ActionFilterAttribute
    {

        private string role = string.Empty;


        public PomeloAuthorize(string role)
        {
            this.role = role;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ITokenService _TokenService = new TokenService();

            var token = _TokenService.GetToken(context.HttpContext);

            if (string.IsNullOrEmpty(token))
            {
                ErrorReturn(context, "token异常");
                return;
            }

            string userId, userRole;
            DateTime expireTime;
            _TokenService.GetUserInfo(token, out userId, out userRole, out expireTime);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                ErrorReturn(context, "用户信息异常");
                return;
            }

            //权限校验
            if (!string.IsNullOrEmpty(role) && !role.Contains(userRole))
            {
                ErrorReturn(context, "用户权限不足");
                return;
            }

            if (expireTime < DateTime.Now)
            {
                ErrorReturn(context, "Token过期");
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
                code = 3,
                data = "",
                message = message,
                time = DateTime.Now.ToString()
            });
        }
    }
}
