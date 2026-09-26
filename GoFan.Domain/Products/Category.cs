using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GoFan.Domain.Products
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? Description { get; set; }
    }
}
