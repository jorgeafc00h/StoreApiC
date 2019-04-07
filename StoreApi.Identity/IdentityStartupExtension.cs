using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using StoreApi.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace StoreApi.Identity
{
    public static class IdentityStartupExtension
    {

        public static void CustomIdentityServerConfiguration(this IServiceCollection services,string connectionString,string migrationsAssembly)
        { 
            //Implement Identity Server 
            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
                //.AddSigningCredential(new RsaSecurityKey())
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
             
              .AddInMemoryIdentityResources(Config.IdentityResources)
              .AddInMemoryClients(Config.Clients)
              .AddInMemoryApiResources(Config.Apis)
              //.AddTestUsers(TestUsers.Users)
              .AddAspNetIdentity<ApplicationUser>();
            
        }

        public static void AddCustomIdentityServer(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        //public static void AddCustomIdentityServerAuthentication(this IServiceCollection services,ILogger _logger)
        //{
        //    //services.AddAuthentication(options =>
        //    //{
        //    //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    //})


        //    services.AddAuthentication("bearer")
        //     .AddIdentityServerAuthentication("bearer", options =>
        //     {
        //         options.Authority = Config.BaseUrl;
        //         options.RequireHttpsMetadata = false;

        //         options.ApiName = "api1";
        //         options.ApiSecret = "secret";

        //         options.JwtBearerEvents = new JwtBearerEvents
        //         {
        //             OnMessageReceived = e =>
        //             {
        //                 _logger.LogTrace("JWT: message received");
        //                 return Task.CompletedTask;
        //             },

        //             OnTokenValidated = e =>
        //             {
        //                 _logger.LogTrace("JWT: token validated");
        //                 return Task.CompletedTask;
        //             },

        //             OnAuthenticationFailed = e =>
        //             {
        //                 _logger.LogTrace("JWT: authentication failed");
        //                 return Task.CompletedTask;
        //             },

        //             OnChallenge = e =>
        //             {
        //                 _logger.LogTrace("JWT: challenge");
        //                 return Task.CompletedTask;
        //             }
        //         };
        //     });

        //    //.AddJwtBearer(options =>
        //    //{
        //    //     // base-address  identityserver
        //    //     options.Authority = Config.BaseUrl;

        //    //     // name of the API resource
        //    //     options.Audience = "api1";

        //    //    options.RequireHttpsMetadata = false;
        //    //});
        //}
    }
}
