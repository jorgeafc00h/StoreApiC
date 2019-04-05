using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class BaseModel
    {

        [Key]
        public int Id { get; set; }
    }
}
