namespace GOFAN.DTOs.Products;

public class CreateProductDto
{
    public int CategoryId { get; set; }

    public required string Name { get; set; }

    public decimal BasePrice { get; set; }

    public string? Description { get; set; }

    public string? TechnicalInfo { get; set; }
}