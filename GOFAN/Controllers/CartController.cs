using System.Security.Claims;
using GoFan.Application.DTOs.Carts;
using GoFan.Application.DTOs.Common;
using GoFan.Application.Exceptions;
using GoFan.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFan.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var customerId = GetCurrentUserId();
        var cart = await _cartService.GetCartByCustomerIdAsync(customerId);
        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        var customerId = GetCurrentUserId();
        var cart = await _cartService.AddToCartAsync(customerId, dto);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Thêm sản phẩm vào giỏ hàng thành công"));
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateCartItemQuantityDto dto)
    {
        var customerId = GetCurrentUserId();
        var cart = await _cartService.UpdateQuantityAsync(customerId, id, dto.Quantity);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Cập nhật số lượng sản phẩm thành công"));
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var customerId = GetCurrentUserId();
        var cart = await _cartService.RemoveItemAsync(customerId, id);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Xóa sản phẩm khỏi giỏ hàng thành công"));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var customerId = GetCurrentUserId();
        await _cartService.ClearCartAsync(customerId);
        var emptyCart = await _cartService.GetCartByCustomerIdAsync(customerId);
        return Ok(ApiResponse<CartDto>.Ok(emptyCart, "Đã làm trống giỏ hàng"));
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncCart([FromBody] SyncCartDto dto)
    {
        var customerId = GetCurrentUserId();
        var cart = await _cartService.SyncCartAsync(customerId, dto);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Đồng bộ giỏ hàng thành công"));
    }

    private int GetCurrentUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }
        return userId;
    }
}
