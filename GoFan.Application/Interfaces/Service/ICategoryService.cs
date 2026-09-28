using GoFan.Domain.Products;

namespace GoFan.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Category> CreateAsync(Category category);

        Task<bool> UpdateAsync(Category category);

        Task<bool> DeleteAsync(int id);
    }
}