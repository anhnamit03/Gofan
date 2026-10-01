using GoFan.Application.DTOs.Common;
using GoFan.Application.DTOs.Products;
using GoFan.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFan.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(ApiResponse<List<ProductDto>>.Ok(products));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found", 404));
        }

        return Ok(ApiResponse<ProductDto>.Ok(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return StatusCode(201, ApiResponse<ProductDto>.Ok(result, "Product created successfully", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(ApiResponse<bool>.Fail("Id không khớp", 400));
        }

        var result = await _productService.UpdateAsync(dto);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Product not found", 404));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Product updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Product not found", 404));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Product deleted successfully"));
    }
}