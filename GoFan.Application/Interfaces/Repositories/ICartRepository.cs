using GoFan.Domain.Carts;

namespace GoFan.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(int customerId);
    Task<Cart> CreateCartAsync(int customerId);
    Task<List<CartItem>> GetCartItemsAsync(int cartId);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
    Task<CartItem?> GetCartItemByProductAsync(int cartId, int productId);
    Task<CartItem> AddCartItemAsync(CartItem item);
    Task<bool> UpdateCartItemAsync(CartItem item);
    Task<bool> RemoveCartItemAsync(CartItem item);
    Task<bool> ClearCartAsync(int cartId);
    Task<List<CartItemAddOn>> GetAddOnsByCartItemIdsAsync(IEnumerable<int> cartItemIds);
    Task AddCartItemAddOnsAsync(IEnumerable<CartItemAddOn> addOns);
    Task RemoveCartItemAddOnsAsync(int cartItemId);
}
