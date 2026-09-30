using GoFan.Application.DTOs.Products;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;

namespace GoFan.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(
        Product product,
        Promotion? promotion)
    {
        decimal? salePrice = null;

        if (promotion != null)
        {
            salePrice =
                product.BasePrice *
                (1 - promotion.DiscountPercent / 100);
        }

        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            Name = product.Name,
            BasePrice = product.BasePrice,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            TechnicalInfo = product.TechnicalInfo,
            Active = product.Active,

            DiscountPercent =
                promotion?.DiscountPercent,

            SalePrice = salePrice,

            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}