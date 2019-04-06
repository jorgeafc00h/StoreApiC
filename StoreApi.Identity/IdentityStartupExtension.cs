using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using StoreApi.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace StoreApi.Identity
{
    public static class IdentityStartupExtension
    {

        public static void CustomIdentityServerConfiguration(this IServiceCollection services)
        { 
            //Implement Identity Server 
            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               //.AddSigningCredential(new RsaSecurityKey())
              .AddInMemoryIdentityResources(Config.IdentityResources)
              .AddInMemoryClients(Config.Clients)
              .AddInMemoryApiResources(Config.Apis)
              .AddTestUsers(TestUsers.Users)
              .AddAspNetIdentity<ApplicationUser>();
            
        }

        public static void AddCustomIdentityServer(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}
