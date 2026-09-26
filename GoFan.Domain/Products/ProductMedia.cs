using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Products
{
    public  class ProductMedia
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public MediaType MediaType { get; set; }
        public required string Url { get; set; }
        public string? AltText { get; set; }
        public int  SortOrder { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
