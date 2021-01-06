using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Pomelo.Web.Middleware
{
    /// <summary>
    /// 错误统一处理返回
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this._next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                if (!context.Response.HasStarted)
                {
                    await _next(context);
                }
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e.Message);

                _logger.LogError($"【状态码】：{context.Response.StatusCode} | 【Url】：{context.Request.Host}{context.Request.Path } |【error】:{e.Message}|{e.InnerException}|{e.StackTrace}");

            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string msg)
        {
            //异常统一输出
            var data = new { code = 1, data = "", message = msg, time = DateTime.Now.ToString() };
            var result = JsonConvert.SerializeObject(data);
            context.Response.ContentType = "application/json;charset=utf-8";
            context.Response.StatusCode = 400;
            return context.Response.WriteAsync(result);
        }
    }
}
