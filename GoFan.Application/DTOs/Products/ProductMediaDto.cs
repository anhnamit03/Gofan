using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Application.DTOs.Products
{
    public class ProductMediaDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; } = null!;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public string MediaType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
