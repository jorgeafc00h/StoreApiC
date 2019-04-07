using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class FilterOptionViewModel
    {

        public string Keywords { get; set; }

        public int Limit { get; set; }

        public int Page { get; set; }

        public bool OrderbyDesc { get; set; } 
    }
}
