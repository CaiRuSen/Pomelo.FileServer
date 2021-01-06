using AspNetCoreRateLimit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System.Threading.Tasks;

namespace Pomelo.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //IWebHost webHost = CreateWebHostBuilder(args).Build();
            //using (var scope = webHost.Services.CreateScope())
            //{
            //    // get the ClientPolicyStore instance
            //    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

            //    // seed client data from appsettings
            //    await clientPolicyStore.SeedAsync();
            //}
            //await webHost.RunAsync();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            //  .UseKestrel(options => {
            //     options.Limits.MaxRequestBodySize = null;
            // })
            .UseNLog();
    }
}
