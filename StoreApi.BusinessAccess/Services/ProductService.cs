using Extensions;
using Microsoft.EntityFrameworkCore;
using Models;
using StoreApi.BusinessAccess.Infrastructure;
using StoreApi.Context;
using StoreApi.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.BusinessAccess.Services
{
    public class ProductService : GenericRepository<StoreDbContext, Product>, IProductService
    {

        public ProductService(StoreDbContext context)
        {
            this.Context = context;
        }

        public Task<List<Product>> GetAll()
        {
            return Context.Products.OrderBy(p => p.Name).ToListAsync();
        }


        public Task<Product> GetProductAsync(int id)
        {
            return Context.Products.FindAsync(id);
        }

        public async Task<Product> AddStock(int productId, int quantity)
        {
            var model = await GetAsync(productId);

            model.AddStock(quantity);

            await InsertOrUpdateAsync(model);

            await Context.WharehouseActivity.AddAsync(new WharehouseActivity
            {
                ProductId = productId,
                ActivityType = WarehouseActivityType.Input,
                Date = DateTime.Now,
                Qty = quantity,
            });

            return model;
        }
         

        public async Task<Product> RemoveStock(int productId, int quantity)
        {
            var model = await GetAsync(productId);

            model.RemoveStock(quantity);

            await InsertOrUpdateAsync(model);

            await Context.WharehouseActivity.AddAsync(new WharehouseActivity{
                ProductId = productId,
                ActivityType = WarehouseActivityType.Output,
                Date = DateTime.Now,
                Qty = quantity,
            });
            

            return model;
        }
         
        public async Task<Product> AddSale(int productId, int quantity)
        {
            var model = await GetAsync(productId);

            model.RemoveStock(quantity);

            await InsertOrUpdateAsync(model);

            await Context.WharehouseActivity.AddAsync(new WharehouseActivity
            {
                ProductId = productId,
                ActivityType = WarehouseActivityType.Sale,
                Date = DateTime.Now,
                Qty = quantity,
                
            });


            return model;
        }

        public async Task<Product> InsertOrUpdateAsync(Product model)
        {
            Context.Entry(model).State = model.Id == 0 ?
                 EntityState.Added : EntityState.Modified;

            var previous = await Context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            var log = new ProductAuditLog()
            {
                ProductId = model.Id,
                UserId = UserId,

            };

            await Context.SaveChangesAsync();

            return model;
        }

        public async Task<SearchResult> Search(string keywords, int limit , int page ,bool orderbydesc =false)
        {
            IQueryable<Product> products =
                orderbydesc ?
                Context.Products.OrderByDescending(p=>p.Name):
                Context.Products.OrderBy(p=>p.Name);

            // small search logic
            keywords.ToSearchKeys().ForEach(k =>
            {
                products = products.Where(p => p.Name.Contains(k));
                products = products.Where(p => p.Description.Contains(k));
            });

            var count = await products.CountAsync();

            var skip = (page - 1) * limit;

            return new SearchResult
            {
                Count = count,
                Page =page,
                Products = await products.Skip(skip).Take(limit).ToListAsync(),

            };

        }

        public Task<SearchResult> Search(FilterOptionViewModel filter)
        {
            return Search(filter.Keywords, filter.Limit, filter.Page, filter.OrderbyDesc);
        }
    }
}
