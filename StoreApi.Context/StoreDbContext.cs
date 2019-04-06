﻿
using Microsoft.EntityFrameworkCore;
using Models;
using StoreApi.Identity.Models;
using System;

namespace StoreApi.Context
{
    public class StoreDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options):base(options)
        {

        }


        public DbSet<Product> Products { get; set; }


        /// <summary>
        /// apply database configurations and complex design with extension classes
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
