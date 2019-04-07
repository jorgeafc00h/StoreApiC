using StoreApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StoreApi.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApi.Context.Data;
using StoreApi.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StoreApi.Identity;
using StoreApi.Infrastructure.Interfaces;
using StoreApi.BusinessAccess.Services;

namespace Test
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {


        /// <summary>
        /// TODO this method still needs a refactoring and cleanup
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<StoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

              //  services.AddIdentity<ApplicationUser, IdentityRole>()
              // .AddEntityFrameworkStores<StoreDbContext>()
              // .AddDefaultTokenProviders();


              //  // call extension method to implement a custom IdentyServer4 for testing purposes
              //  services.AddIdentityServer()
              // .AddDeveloperSigningCredential()
              //  //.AddSigningCredential(new RsaSecurityKey())
              //.AddInMemoryIdentityResources(Config.IdentityResources)
              //.AddInMemoryClients(Config.Clients)
              //.AddInMemoryApiResources(Config.Apis)
              ////.AddTestUsers(TestUsers.Users)
              //.AddAspNetIdentity<ApplicationUser>();

               // services.AddMvcCore().AddAuthorization().AddJsonFormatters();

               // services.AddAuthentication(options =>
               // {
               //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               // })
               //.AddJwtBearer(options =>
               //{
               //// base-address  identityserver
               //options.Authority = Config.BaseUrl;

               //// name of the API resource
               //options.Audience = "api1";

               //    options.RequireHttpsMetadata = false;
               //});

               // //services.AddCustomIdentityServerAuthentication(_logger);

               // services.AddCors(options =>
               // {
               //     // this defines a CORS policy called "default"
               //     options.AddPolicy("default", policy =>
               //     {
               //         policy.AllowAnyOrigin();
               //     });
               // });

                // Implement Repositories
                services.AddScoped<IProductService, ProductService>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<StoreDbContext>();

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    context.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with some specific test data.
                        new StoreContextSeed().SeedInMemoryDataForMock(context, sp,migrateDatabase : false).Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }

 
             
    }
}
