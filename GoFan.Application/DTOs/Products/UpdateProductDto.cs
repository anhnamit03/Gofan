namespace GoFan.Application.DTOs.Products;

public class UpdateProductDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public required string Name { get; set; }

    public decimal BasePrice { get; set; }

    public string? Description { get; set; }

    public string? TechnicalInfo { get; set; }

    public bool Active { get; set; }
}