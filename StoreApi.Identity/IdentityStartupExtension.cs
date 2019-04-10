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
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace StoreApi.Identity
{
    public static class IdentityStartupExtension
    {

        public static void CustomIdentityServerConfiguration(this IServiceCollection services,string connectionString,string migrationsAssembly)
        {

            //Implement Identity Server 
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                
                
                //.AddSigningCredential(cert)
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
              .AddTestUsers(TestUsers.Users)
              .AddAspNetIdentity<ApplicationUser>();
            
        }

        public static void AddCustomIdentityServer(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        private static SigningCredentials CreateSigningCredential()
        {
            var credentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.RsaSha256Signature);

            return credentials;
        }
        private static RSACryptoServiceProvider GetRSACryptoServiceProvider()
        {
            return new RSACryptoServiceProvider(2048);
        }
        private static SecurityKey GetSecurityKey()
        {
            var key = "Test";

            var param = new RSAParameters
            {
                D = Encoding.ASCII.GetBytes(key) ,
                
            };


            return IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey(param, "2a2467e4a208110eb83896572a7c10af");
        }



    }
}
