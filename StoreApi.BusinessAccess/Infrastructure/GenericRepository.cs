using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace StoreApi.BusinessAccess.Infrastructure
{
    public abstract class BaseRepository<TContext, Tmodel> : GenericRepository<TContext, Tmodel>
        where TContext : DbContext
        where Tmodel : BaseModel
    {

        public async Task<Tmodel> InsertOrUpdateAsync(Tmodel model)
        {
            Context.Entry(model).State = model.Id == 0 ?
                EntityState.Added : EntityState.Modified;
  
            await Context.SaveChangesAsync();

            return model;
        }


    }


    public abstract class GenericRepository<TContext, Tmodel>
      where TContext : DbContext
      where Tmodel : class
    {

        public Task<Tmodel> GetAsync(int id)
        {
            return Context.Set<Tmodel>().FindAsync(id);
        }


        public async Task<Tmodel> CreateAsync(Tmodel model)
        {
            Context.Entry(model).State = EntityState.Added;

            await Context.SaveChangesAsync();

            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var model = await Context.FindAsync<Tmodel>(id);

            Context.Entry(model).State = EntityState.Deleted;

            await Context.SaveChangesAsync();
        }

        public Task<List<Tmodel>> GetAllAsync()
        {
            return Context.Set<Tmodel>().AsTracking().ToListAsync();
        }

        public Task<int> CountAsync()
        {
            return Context.Set<Tmodel>().CountAsync();
        }

        public IQueryable<Tmodel> AsNotracking()
        {
            return Context.Set<Tmodel>().AsNoTracking();
        }



        static string[] PotentialIdClaim = { System.Security.Claims.ClaimTypes.NameIdentifier, "sub" };


        protected string UserId
        {
            get
            {


                var userIdClaim = ClaimsPrincipal.Current.FindFirst(c => PotentialIdClaim.Contains(c.Type));

                //if (userIdClaim == null) throw new Exception("Authentication Failed, verify identity Server Settings");

                return userIdClaim != null ? userIdClaim.Value : "";
            }
        }


        public TContext Context;
    }
}
