using Contoso.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Contoso.Web
{
    public class Program
    {
        public static void Main(string[] args) => 
            CreateHostBuilder(args)
                .Build()
                .SeedDatabase()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
