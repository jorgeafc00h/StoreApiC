using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace StoreApi.Context
{
    public class StoreDbContext : DbContext
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

        }
    }
}
