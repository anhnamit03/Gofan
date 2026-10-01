using GoFan.Application.DTOs.Carts;

namespace GoFan.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartDto> GetCartByCustomerIdAsync(int customerId);
    Task<CartDto> AddToCartAsync(int customerId, AddToCartDto dto);
    Task<CartDto> UpdateQuantityAsync(int customerId, int cartItemId, int quantity);
    Task<CartDto> RemoveItemAsync(int customerId, int cartItemId);
    Task<bool> ClearCartAsync(int customerId);
    Task<CartDto> SyncCartAsync(int customerId, SyncCartDto dto);
}
