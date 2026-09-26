using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Products
{
    public class ProductAddOn
    {
        public int Id { get; set; }
        public int MainProductId { get; set; }
        public int AddOnProductId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
