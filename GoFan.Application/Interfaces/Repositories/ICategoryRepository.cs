using GoFan.Domain.Products;

namespace GoFan.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Category> AddAsync(Category category);

        Task<bool> UpdateAsync(Category category);

        Task<bool> DeleteAsync(Category category);
    }
}