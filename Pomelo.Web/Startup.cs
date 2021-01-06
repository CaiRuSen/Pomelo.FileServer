using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models; 
using Pomelo.Web.Middleware; 
using AspNetCoreRateLimit;

namespace Pomelo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //跨域设置
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
 

            //加载限流配置
            services.AddOptions();
            services.AddMemoryCache();
            //加载ClientRateLimiting配置
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            //  services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            //注入计数器和规则存储
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

         
            // clientId / clientIp解析器使用它。
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //配置（计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


            services.AddControllers(); 

            //Swagger
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pomelo API", Version = "v1" });
                    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）

                    c.IncludeXmlComments(Path.Combine(basePath, "Pomelo.Web.xml"));

                });
              
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pomelo API");
                c.RoutePrefix = "doc";
            });

            PomeloProvider.ServiceProvider = app.ApplicationServices; 

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseClientRateLimiting(); 
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
