namespace GoFan.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public string? TechnicalInfo { get; set; }

    public bool Active { get; set; }

    // Promotion hiện tại
    public GoFan.Application.DTOs.Promotions.PromotionDto? Promotion { get; set; }

    // Hình ảnh / video
    public List<ProductMediaDto> Medias { get; set; } = new();

    // Sản phẩm mua kèm
    public List<ProductAddOnDto> AddOns { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}