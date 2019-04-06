using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StoreApi.Context;
using StoreApi.Context.Data;
using WebHost.Customization;
using Microsoft.Extensions.DependencyInjection;

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

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();

        public static IWebHost BuildWebHost(string[] args) =>
           Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
               .Build();
    }
}
