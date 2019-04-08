using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StoreApi.Context;
using StoreApi.Context.Data;
using StoreApi.Identity;
using WebHost.Customization;

namespace StoreApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<StoreDbContext>((context, services) =>
                {
                    new StoreContextSeed().SeedAsync(context, services).Wait();
                })
                .Run();
                
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        public static IWebHost BuildWebHost(string[] args) =>
           Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            })
            .UseSetting("https_port", "44373")
            .UseUrls("http://localhost:49555/",Config.BaseUrl)
                .UseStartup<Startup>()
               .Build();
    }
}
