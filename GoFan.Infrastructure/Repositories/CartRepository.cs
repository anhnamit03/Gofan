using GoFan.Application.Interfaces.Repositories;
using GoFan.Domain.Carts;
using GoFan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoFan.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Cart> CreateCartAsync(int customerId)
    {
        var cart = new Cart
        {
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<List<CartItem>> GetCartItemsAsync(int cartId)
    {
        return await _context.CartItems
            .Where(i => i.CartId == cartId)
            .OrderByDescending(i => i.UpdatedAt)
            .ToListAsync();
    }

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(i => i.Id == cartItemId);
    }

    public async Task<CartItem?> GetCartItemByProductAsync(int cartId, int productId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);
    }

    public async Task<CartItem> AddCartItemAsync(CartItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.CartItems.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> UpdateCartItemAsync(CartItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveCartItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int cartId)
    {
        var items = await _context.CartItems.Where(i => i.CartId == cartId).ToListAsync();
        if (items.Count != 0)
        {
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        return true;
    }

    public async Task<List<CartItemAddOn>> GetAddOnsByCartItemIdsAsync(IEnumerable<int> cartItemIds)
    {
        var ids = cartItemIds.Distinct().ToList();
        if (ids.Count == 0) return new List<CartItemAddOn>();

        return await _context.CartItemAddOns
            .Where(a => ids.Contains(a.CartItemId))
            .ToListAsync();
    }

    public async Task AddCartItemAddOnsAsync(IEnumerable<CartItemAddOn> addOns)
    {
        var list = addOns.ToList();
        if (list.Count != 0)
        {
            await _context.CartItemAddOns.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveCartItemAddOnsAsync(int cartItemId)
    {
        var addOns = await _context.CartItemAddOns
            .Where(a => a.CartItemId == cartItemId)
            .ToListAsync();

        if (addOns.Count != 0)
        {
            _context.CartItemAddOns.RemoveRange(addOns);
            await _context.SaveChangesAsync();
        }
    }
}
