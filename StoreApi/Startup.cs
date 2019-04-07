using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreApi.BusinessAccess.Services;
using StoreApi.Context;
using StoreApi.Identity;
using StoreApi.Identity.Models;
using StoreApi.Infrastructure.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace StoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration,ILogger<Startup> logger)
        {
            Configuration = configuration;
            this._logger = logger;
        }

        public IConfiguration Configuration { get; }

        private readonly ILogger<Startup> _logger;

        // This method gets called by the runtime. Use this method to add services to the container or web app.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionstring = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(StoreDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(connectionstring, sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval

            });

            // TO Avoid Circular References
            services.AddMvc()
                
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StoreApiCoding", Version = "v1" });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<StoreDbContext>()
                .AddDefaultTokenProviders();

            // call extension method to implement a custom IdentyServer4 Implementation. 
            services.CustomIdentityServerConfiguration(connectionstring,migrationsAssembly);

            services.AddMvcCore().AddAuthorization().AddJsonFormatters();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
                // base-address  identityserver
                options.Authority = Config.BaseUrl;

                // name of the API resource
                options.Audience = "api1";

               options.RequireHttpsMetadata = false;
           });

            //services.AddCustomIdentityServerAuthentication(_logger);

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin();
                });
            });

            // Implement Repositories
            services.AddScoped<IProductService, ProductService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();

          
            app.UseAuthentication();
            app.AddCustomIdentityServer();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreApiCoding V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
