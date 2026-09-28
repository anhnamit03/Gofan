using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace GoFan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var result = await _categoryService.CreateAsync(category);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("Id không khớp");
            }

            var result = await _categoryService.UpdateAsync(category);

            if (!result)
            {
                return NotFound("Category not found");
            }

            return Ok("Category updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (!result)
            {
                return NotFound("Category not found");
            }

            return Ok("Category deleted successfully");
        }
    }
}