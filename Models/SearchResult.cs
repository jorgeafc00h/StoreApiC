using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class SearchResult
    {
        public List<Product> Products { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }

    }
}
