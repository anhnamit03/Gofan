using GoFan.Application.DTOs.Common;
using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace GoFan.Controllers;

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
        return Ok(ApiResponse<List<Category>>.Ok(categories));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(ApiResponse<Category>.Fail("Category not found", 404));
        }

        return Ok(ApiResponse<Category>.Ok(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        var result = await _categoryService.CreateAsync(category);
        return StatusCode(201, ApiResponse<Category>.Ok(result, "Category created successfully", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest(ApiResponse<bool>.Fail("Id không khớp", 400));
        }

        var result = await _categoryService.UpdateAsync(category);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Category not found", 404));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Category updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Category not found", 404));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Category deleted successfully"));
    }
}