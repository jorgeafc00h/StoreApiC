using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using StoreApi.Infrastructure.Interfaces;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        public ProductsController(IProductService repo)
        {
            this.Service = repo;
        }

        /// <summary>
        /// return a list of all available products ordered by name
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet(Name ="GetAll")]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return  Ok(await Service.GetAll());
        }

       
       /// <summary>
       /// returns a product by Id 
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        [HttpGet("{id}",Name = "GetById")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {

            var product = await Service.GetAsync(id);

            if (product != null)
                return Ok(product);
            else
                return NotFound();

        }

        /// <summary>
        /// returns a product by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("productlog/{id}", Name = "GetProductLog")]
        [ProducesResponseType(typeof(IEnumerable<ProductAuditLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetProductLog(int id)
        {

            var logs = await Service.GetProductLogsAsync(id);

            if (logs != null || logs.Any())
                return Ok(logs);
            else
                return NotFound();
        }


        /// <summary>
        /// Creates a product 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/
        [HttpPost(Name ="Create")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] Product model)
        {
          return Ok(await Service.InsertOrUpdateAsync(model));
        }


        /// <summary>
        /// performs a product search with default order by name 
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="orderbydesc"></param>
        /// <returns></returns>
        // GET api/values
        [HttpGet("search/{keywords}", Name = "Search")]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string keywords, int limit=30, int page=1, bool orderbydesc = false)
        {
            return Ok(await Service.Search(keywords, limit, page, orderbydesc));
        }


        /// <summary>
        /// Returns a paged result from specific search with keywords ordered 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("search/{filter}",Name = "Custom Search")]
        [ProducesResponseType(typeof(SearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromBody] FilterOptionViewModel filter)
        {
            if (filter.Keywords.Contains("XYZ Widget"))
            {
                return BadRequest();
            }

            return Ok(await Service.Search(filter));

        }
        /// <summary>
        /// Increments the quantity of a particular item in inventory.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("AddStock/{productId}", Name = "AddStock")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> AddStock(int productId, int quantity)
        {
            var product = await Service.AddStock(productId, quantity);

            if (product != null) return Ok(product);

            return NotFound();

        }


        /// <summary>
        /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
		/// been breached. If so, a RestockRequest is generated in CheckThreshold. 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        // PUT api/
        [HttpPut("RemoveStock/{productId}", Name = "RemoveStock")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> RemoveStock(int productId, int quantity)
        {
            var product = await Service.RemoveStock(productId, quantity);

            if(product!=null) return Ok(product);

            return NotFound();

        }

        /// <summary>
        /// a common sale 
        /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
        /// been breached. If so, a RestockRequest is generated in CheckThreshold. 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("AddSale/{productId}", Name = "AddSale")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> AddSale(int productId, int quantity)
        {
            await Service.AddSale(productId, quantity);

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await Service.DeleteAsync(id);

            return Ok();
        }
         

        private IProductService Service { get; set; }
    }
}