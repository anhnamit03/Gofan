using GoFan.Application.DTOs.Products;

namespace GoFan.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(int id);

    Task<ProductDto> CreateAsync(CreateProductDto dto);

    Task<bool> UpdateAsync(UpdateProductDto dto);

    Task<bool> DeleteAsync(int id);
}