using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.Infrastructure.Interfaces
{
    public interface IProductService
    {
        Task<SearchResult> Search(string keywords, int limit, int page, bool orderbydesc = false);

        Task<SearchResult> Search(FilterOptionViewModel filter);

        Task<Product> GetAsync(int id);

        Task<List<Product>> GetAll();

        Task<Product> CreateAsync(Product model);

        Task<Product> InsertOrUpdateAsync(Product model);

        Task<Product> AddStock(int productId,int quantity);

        Task<Product> RemoveStock(int productId, int quantity);

        Task<Product> AddSale(int productId, int quantity);

        Task DeleteAsync(int id);
        Task<Product> GetProductAsync(int id);

        Task<IEnumerable<ProductAuditLog>> GetProductLogsAsync(int id);

        Task<int> CountAsync();

        IQueryable<Product> AsNotracking();
    }
}
