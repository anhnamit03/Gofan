using GoFan.Application.DTOs.Products;
using GoFan.Application.Interfaces.Repositories;
using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Products;

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

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            Name = product.Name,
            BasePrice = product.BasePrice,
            Description = product.Description,
            TechnicalInfo = product.TechnicalInfo,
            Active = product.Active,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        }).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            Name = product.Name,
            BasePrice = product.BasePrice,
            Description = product.Description,
            TechnicalInfo = product.TechnicalInfo,
            Active = product.Active,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            BasePrice = dto.BasePrice,
            Description = dto.Description,
            TechnicalInfo = dto.TechnicalInfo,
            SKU = Guid.NewGuid().ToString("N")[..12],
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);

        return new ProductDto
        {
            Id = createdProduct.Id,
            CategoryId = createdProduct.CategoryId,
            SKU = createdProduct.SKU,
            Name = createdProduct.Name,
            BasePrice = createdProduct.BasePrice,
            Description = createdProduct.Description,
            TechnicalInfo = createdProduct.TechnicalInfo,
            Active = createdProduct.Active,
            CreatedAt = createdProduct.CreatedAt,
            UpdatedAt = createdProduct.UpdatedAt
        };
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
        existingProduct.Description = dto.Description;
        existingProduct.TechnicalInfo = dto.TechnicalInfo;
        existingProduct.Active = dto.Active;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        return await _productRepository.UpdateAsync(existingProduct);
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