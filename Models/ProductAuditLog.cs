using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProductAuditLog:BaseModel
    {

        public Product Product { get; set; }

        public int ProductId { get; set; }

        
    }
}
