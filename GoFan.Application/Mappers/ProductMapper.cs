using GoFan.Application.DTOs.Products;
using GoFan.Application.DTOs.Promotions;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;
using System.Linq;

namespace GoFan.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(
        Product product,
        Promotion? promotion,
        IEnumerable<ProductMedia>? medias = null,
        IEnumerable<ProductAddOn>? addOns = null)
    {
        decimal? discountPrice = null;

        PromotionDto? promoDto = null;
        if (promotion != null)
        {
            discountPrice = product.BasePrice * (1 - promotion.DiscountPercent / 100);
            promoDto = new PromotionDto
            {
                Id = promotion.Id,
                Name = promotion.Name,
                DiscountPercent = promotion.DiscountPercent,
                StartAt = promotion.StartAt,
                EndAt = promotion.EndAt,
                Active = promotion.Active
            };
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            Name = product.Name,
            BasePrice = product.BasePrice,
            DiscountPrice = discountPrice,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            TechnicalInfo = product.TechnicalInfo,
            Active = product.Active,
            Promotion = promoDto,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        if (medias != null)
        {
            dto.Medias = medias.Select(m => new ProductMediaDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                Url = m.Url,
                AltText = m.AltText,
                SortOrder = m.SortOrder,
                MediaType = m.MediaType.ToString(),
                CreatedAt = m.CreatedAt
            }).OrderBy(m => m.SortOrder).ToList();
        }

        if (addOns != null)
        {
            dto.AddOns = addOns.Select(a => new ProductAddOnDto
            {
                Id = a.Id,
                AddOnProductId = a.AddOnProductId,
                AddOnProductName = string.Empty,
                SKU = null,
                UnitPrice = 0m
            }).ToList();
        }

        return dto;
    }
}
