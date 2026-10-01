using GoFan.Domain.Products;
using GoFan.Domain.Promotions;

namespace GoFan.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);

    Task<Promotion?> GetActivePromotionAsync(
        int productId,
        DateTime now);

    Task<Dictionary<int, Promotion>> GetActivePromotionsAsync(
        IEnumerable<int> productIds,
        DateTime now);

    Task<List<ProductMedia>> GetMediasAsync(int productId);

    Task<List<ProductAddOn>> GetAddOnsAsync(int productId);

    Task<Product> AddAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(Product product);
}