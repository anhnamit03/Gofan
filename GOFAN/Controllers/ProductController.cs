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

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Id không khớp");
        }

        var result = await _productService.UpdateAsync(dto);

        if (!result)
        {
            return NotFound("Product not found");
        }

        return Ok("Product updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result)
        {
            return NotFound("Product not found");
        }

        return Ok("Product deleted successfully");
    }
}