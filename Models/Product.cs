using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Product : BaseModel
    {

        public int Qty { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

    }
}
