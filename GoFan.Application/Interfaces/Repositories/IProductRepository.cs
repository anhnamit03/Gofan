using GoFan.Domain.Products;
using GoFan.Domain.Promotions;

namespace GoFan.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<Promotion?> GetActivePromotionAsync(
        int productId,
        DateTime now);

    Task<Product> AddAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(Product product);
}