using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StoreApi.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
