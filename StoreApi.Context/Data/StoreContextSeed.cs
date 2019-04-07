
using Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using StoreApi.Identity.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreApi.Context.Data
{
    public class StoreContextSeed
    {
        public async Task SeedAsync(StoreDbContext context, IServiceProvider services, int? retry = 0)
        {

            int retryForAvaiability = retry.Value;

            var env = services.GetService<IHostingEnvironment>();
            var logger = services.GetService<ILogger<StoreDbContext>>();

            //Apply migrations if exists or automatically
            context.Database.Migrate();

            try
            {
                var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

                var contentRootPath = env.ContentRootPath;
                var webroot = env.WebRootPath;
                using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                { 
                    if (!await context.Users.AnyAsync())
                    {
                        var users = GetUsersFromFileOrDefaults(contentRootPath, logger);
                        context.Users.AddRange(users);

                        await context.SaveChangesAsync();

                        foreach(var user in users)
                        {
                            var result = await userMgr.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, user.Role));
                            
                        }

                    }

                    if(!await context.Products.AnyAsync())
                    {
                        context.Products.AddRange(GetDumpProducts());

                        await context.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(StoreDbContext));

                    await SeedAsync(context, services, retryForAvaiability);
                }
            }
        }

        #region Custom users initialization from file or hardcoded asp.net identity core

        private IEnumerable<ApplicationUser> GetUsersFromFileOrDefaults(string contentRootPath, ILogger logger)
        {
            string csvFileUsers = Path.Combine(contentRootPath, "Setup", "Users.csv");

            if (!File.Exists(csvFileUsers))
            {
                return GetDefaultUser();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = {
                    "email", "lastname", "name","username",
                    "normalizedemail", "normalizedusername", "password","role"
                };
                csvheaders = GetHeaders(requiredHeaders, csvFileUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

                return GetDefaultUser();
            }

            List<ApplicationUser> users = File.ReadAllLines(csvFileUsers)
                        .Skip(1) // skip header column
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => CreateApplicationUser(column, csvheaders))
                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null)
                        .ToList();

            return users;
        }

        private ApplicationUser CreateApplicationUser(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            var user = new ApplicationUser
            {
                Email = column[Array.IndexOf(headers, "email")].Trim('"').Trim(),
                
                Id = Guid.NewGuid().ToString(),
                LastName = column[Array.IndexOf(headers, "lastname")].Trim('"').Trim(),
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                PhoneNumber = column[Array.IndexOf(headers, "phonenumber")].Trim('"').Trim(),
                UserName = column[Array.IndexOf(headers, "username")].Trim('"').Trim(),
                NormalizedEmail = column[Array.IndexOf(headers, "normalizedemail")].Trim('"').Trim(),
                NormalizedUserName = column[Array.IndexOf(headers, "normalizedusername")].Trim('"').Trim(),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                PasswordHash = column[Array.IndexOf(headers, "password")].Trim('"').Trim(), // Note: This is the password
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            return user;
        }

        private IEnumerable<ApplicationUser> GetDefaultUser()
        {
            var admin =new ApplicationUser()
            {
              
                Id = Guid.NewGuid().ToString(),
                LastName = "Demo AdminLastName",
                Name = "Admin DemoUser",
                PhoneNumber = "1234567890",
                UserName = "admin@yopmail.com",
                NormalizedEmail = "ADMIN@YOPMAIL.COM",
                NormalizedUserName = "ADMIN@YOPMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Role ="Admin",
            };

            var user = new ApplicationUser()
            {

                Id = Guid.NewGuid().ToString(),
                LastName = "User",
                Name = "DemoUser",
                PhoneNumber = "1234567890",
                UserName = "user@yopmail.com",
                NormalizedEmail = "user@YOPMAIL.COM",
                NormalizedUserName = "user@YOPMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Role = "User",
            };

           user.PasswordHash= admin.PasswordHash = _passwordHasher.HashPassword(admin, "1234567");

            return new List<ApplicationUser>()
            {
                admin,user
            };
        }

        static string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        #endregion

        #region Product Data

        public List<Product> GetDumpProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Name ="Apple slices with almond butter.",
                    Description = "Description Apple slices with almond butter.",
                    AvailableStock=35,
                    RestockThreshold =5,
                    MaxStockThreshold = 50,
                    Price = 5


                },
                new Product()
                {
                    Name ="Salad Cookie",
                    Description = "Description Salad Cookie",
                    AvailableStock=25,
                    RestockThreshold =5,
                    MaxStockThreshold = 50,
                    Price =  (decimal) 1.50,
                },
                new Product()
                {
                    Name ="Bubblegum",
                    Description = "A type of chewing gum, designed to be inflated out of the mouth as a bubble. As with chewing gum, the product is made from chicle and is available in various flavors.",
                    AvailableStock=100,
                    RestockThreshold =25,
                    MaxStockThreshold = 150,
                    Price =  (decimal) 0.25,
                },
                 new Product()
                {
                    Name ="Chocolate",
                    Description = "Chocolate has been used as a drink for nearly all of its history, and has become one of the most popular food types ",
                    AvailableStock=95,
                    RestockThreshold =25,
                    MaxStockThreshold = 100,
                    Price =  (decimal) 1.25,
                },
                 new Product()
                {
                    Name ="Marshmallow",
                    Description = "Marshmallow bag 25 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Chocolate rugelach",
                    Description = "Chocolate rugelach 10 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Chocolate truffle",
                    Description = "Chocolate truffle 5 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Fudge",
                    Description = "Fudge 10 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Fudge",
                    Description = "Fudge 10 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Fudge",
                    Description = "Typically sweet and rich, it's prepared by mixing sugar, butter, and milk,  10 small units",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Geplak",
                    Description = "Made from equal parts coarsely grated coconut and sugar, in equal amounts, often colored brightly. Other variations exist.",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                 new Product()
                {
                    Name ="Grass jelly",
                    Description = "A jelly-like dessert prepared by boiling the aged and slightly oxidized stalks and leaves of Mesona chinensis",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                   new Product()
                {
                    Name ="Nougat",
                    Description = "A variety of similar traditional confectioneries made with sugar and/or honey, roasted nuts (almonds, walnuts, pistachios, hazelnuts, and recently macadamia nuts are common)",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                    new Product()
                {
                    Name ="Chocolate chip cookie",
                    Description = "Chocolate chip cookie",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
                     new Product()
                {
                    Name ="Ginger snaps",
                    Description = "Ginger snaps",
                    AvailableStock=90,
                    RestockThreshold =15,
                    MaxStockThreshold = 120,
                    Price =  (decimal) 3.50,
                },
            };
        }


        #endregion

        #region private properties

        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
        #endregion
    }
}
