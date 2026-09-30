using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Products;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public required string SKU { get; set; }

    public required string Name { get; set; }

    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public string? TechnicalInfo { get; set; }

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}