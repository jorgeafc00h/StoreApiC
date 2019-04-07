using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Product : BaseModel
    {

      

        public string Name { get; set; }

        public string Description { get; set; }


        public string Sku { get; set; }

        public decimal Price { get; set; }

        public int AvailableStock { get; set; }

        // Available stock at which we should reorder
        public int RestockThreshold { get; set; }


        // Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
        public int MaxStockThreshold { get; set; }

        /// <summary>
		/// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
		/// been breached. If so, a RestockRequest is generated in CheckThreshold. 
		/// </summary>
		/// <param name="quantityDesired"></param>
		/// <returns>int: Returns the number actually removed from stock. </returns>
		/// 
		public int RemoveStock(int quantityDesired)
        {
            if (AvailableStock == 0)
            {
                throw new ProductDomainException($"Empty stock, product item {Name} is sold out");
            }

            if (quantityDesired <= 0)
            {
                throw new ProductDomainException($"Item units desired should be greater than zero");
            }

            int removed = Math.Min(quantityDesired, this.AvailableStock);

            this.AvailableStock -= removed;

            return removed;
        }

        /// <summary>
        /// Increments the quantity of a particular item in inventory.
        /// <param name="quantity"></param>
        /// <returns>int: Returns the quantity that has been added to stock</returns>
        /// </summary>
        public int AddStock(int quantity)
        {
            int original = this.AvailableStock;

            // The quantity that the client is trying to add to stock is greater than what can be physically accommodated in the Warehouse
            if ((this.AvailableStock + quantity) > this.MaxStockThreshold)
            {
                // For now, this method only adds new units up maximum stock threshold. In an expanded version of this application, we
                //could include tracking for the remaining units and store information about overstock elsewhere. 
                this.AvailableStock += (this.MaxStockThreshold - this.AvailableStock);
            }
            else
            {
                this.AvailableStock += quantity;
            }
             

            return this.AvailableStock - original;
        }

    }


}
