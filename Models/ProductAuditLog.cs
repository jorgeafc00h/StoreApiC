﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProductAuditLog:BaseModel
    {

        public DateTime Date { get; set; } = DateTime.Now;

        public Product Product { get; set; }

        public int ProductId { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }
    }
}
