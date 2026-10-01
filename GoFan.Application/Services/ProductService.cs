using GoFan.Application.DTOs.Products;
using GoFan.Application.Interfaces.Repositories;
using GoFan.Application.Interfaces.Services;
using GoFan.Application.Mappers;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;

namespace GoFan.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        if (products.Count == 0)
        {
            return new List<ProductDto>();
        }

        var now = DateTime.UtcNow;
        var productIds = products.Select(p => p.Id);
        var promotionsMap = await _productRepository.GetActivePromotionsAsync(productIds, now);

        var result = new List<ProductDto>(products.Count);
        foreach (var product in products)
        {
            promotionsMap.TryGetValue(product.Id, out var promotion);
            result.Add(ProductMapper.ToDto(product, promotion));
        }

        return result;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var promotion = await _productRepository.GetActivePromotionAsync(product.Id, now);
        var medias = await _productRepository.GetMediasAsync(product.Id);
        var addOns = await _productRepository.GetAddOnsAsync(product.Id);

        var dto = ProductMapper.ToDto(product, promotion, medias, addOns);

        // fill addon product details using batch lookup
        if (dto.AddOns != null && dto.AddOns.Count > 0)
        {
            var addOnIds = dto.AddOns.Select(a => a.AddOnProductId).Distinct().ToList();
            var addOnProducts = await _productRepository.GetByIdsAsync(addOnIds);
            var dict = addOnProducts.ToDictionary(p => p.Id);

            foreach (var a in dto.AddOns)
            {
                if (dict.TryGetValue(a.AddOnProductId, out var prod))
                {
                    a.AddOnProductName = prod.Name;
                    a.SKU = prod.SKU;
                    a.UnitPrice = prod.BasePrice;
                }
            }
        }

        return dto;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            BasePrice = dto.BasePrice,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description,
            TechnicalInfo = dto.TechnicalInfo,

            SKU = Guid.NewGuid().ToString("N")[..12],

            Active = true,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProduct =
            await _productRepository.AddAsync(product);

        return ProductMapper.ToDto(
            createdProduct,
            null);
    }

    public async Task<bool> UpdateAsync(UpdateProductDto dto)
    {
        var existingProduct =
            await _productRepository.GetByIdAsync(dto.Id);

        if (existingProduct == null)
        {
            return false;
        }

        existingProduct.CategoryId = dto.CategoryId;
        existingProduct.Name = dto.Name;
        existingProduct.BasePrice = dto.BasePrice;
        existingProduct.ImageUrl = dto.ImageUrl;
        existingProduct.Description = dto.Description;
        existingProduct.TechnicalInfo = dto.TechnicalInfo;
        existingProduct.Active = dto.Active;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        return await _productRepository.UpdateAsync(
            existingProduct);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return false;
        }

        return await _productRepository.DeleteAsync(product);
    }
}