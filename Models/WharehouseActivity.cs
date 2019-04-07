using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class WharehouseActivity : BaseModel
    {

        public WharehouseActivity()
        {

        }

        public DateTime Date { get; set; } = DateTime.Now;

        public Product Product { get; set; }

        public int ProductId { get; set; }

        public int Qty { get; set; }

        public WarehouseActivityType ActivityType { get; set; }


        public string UserId { get; set; }
    }
}
