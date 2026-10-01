using System;

namespace GoFan.Application.DTOs.Products;

public class ProductAddOnDto
{
    public int Id { get; set; }
    public int AddOnProductId { get; set; }
    public string AddOnProductName { get; set; } = null!;
    public string? SKU { get; set; }
    public decimal UnitPrice { get; set; }
}
