using GoFan.Domain.Products;

namespace GoFan.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> AddAsync(Product product);

        Task<bool> UpdateAsync(Product product);

        Task<bool> DeleteAsync(Product product);
    }
}