using GoFan.Application.Interfaces.Repositories;
using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Products;

namespace GoFan.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            return await _categoryRepository.AddAsync(category);
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existingCategory =
                await _categoryRepository.GetByIdAsync(category.Id);

            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Active = category.Active;
            existingCategory.UpdateAt = DateTime.UtcNow;

            return await _categoryRepository.UpdateAsync(existingCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category =
                await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            return await _categoryRepository.DeleteAsync(category);
        }
    }
}