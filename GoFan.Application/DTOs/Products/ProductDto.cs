namespace GoFan.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public string? TechnicalInfo { get; set; }

    public bool Active { get; set; }

    // Promotion hiện tại
    public decimal? DiscountPercent { get; set; }

    public decimal? SalePrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}