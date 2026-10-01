using GoFan.Application.DTOs.Carts;
using GoFan.Application.Exceptions;
using GoFan.Application.Interfaces.Repositories;
using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Carts;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;

namespace GoFan.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartByCustomerIdAsync(int customerId)
    {
        var cart = await GetOrCreateCartAsync(customerId);
        return await BuildCartDtoAsync(cart);
    }

    public async Task<CartDto> AddToCartAsync(int customerId, AddToCartDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null || !product.Active)
        {
            throw new NotFoundException("Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
        }

        var cart = await GetOrCreateCartAsync(customerId);
        var existingItem = await _cartRepository.GetCartItemByProductAsync(cart.Id, dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            await _cartRepository.UpdateCartItemAsync(existingItem);

            if (dto.AddOnProductIds != null && dto.AddOnProductIds.Count != 0)
            {
                await AddOrUpdateAddOnsAsync(existingItem.Id, dto.AddOnProductIds);
            }
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
            };
            var createdItem = await _cartRepository.AddCartItemAsync(newItem);

            if (dto.AddOnProductIds != null && dto.AddOnProductIds.Count != 0)
            {
                await AddOrUpdateAddOnsAsync(createdItem.Id, dto.AddOnProductIds);
            }
        }

        return await BuildCartDtoAsync(cart);
    }

    public async Task<CartDto> UpdateQuantityAsync(int customerId, int cartItemId, int quantity)
    {
        var cart = await GetOrCreateCartAsync(customerId);
        var item = await _cartRepository.GetCartItemByIdAsync(cartItemId);

        if (item == null || item.CartId != cart.Id)
        {
            throw new NotFoundException("Mục giỏ hàng không tồn tại hoặc không thuộc quyền sở hữu.");
        }

        if (quantity <= 0)
        {
            await _cartRepository.RemoveCartItemAddOnsAsync(item.Id);
            await _cartRepository.RemoveCartItemAsync(item);
        }
        else
        {
            item.Quantity = quantity;
            await _cartRepository.UpdateCartItemAsync(item);
        }

        return await BuildCartDtoAsync(cart);
    }

    public async Task<CartDto> RemoveItemAsync(int customerId, int cartItemId)
    {
        var cart = await GetOrCreateCartAsync(customerId);
        var item = await _cartRepository.GetCartItemByIdAsync(cartItemId);

        if (item != null && item.CartId == cart.Id)
        {
            await _cartRepository.RemoveCartItemAddOnsAsync(item.Id);
            await _cartRepository.RemoveCartItemAsync(item);
        }

        return await BuildCartDtoAsync(cart);
    }

    public async Task<bool> ClearCartAsync(int customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart != null)
        {
            await _cartRepository.ClearCartAsync(cart.Id);
        }
        return true;
    }

    public async Task<CartDto> SyncCartAsync(int customerId, SyncCartDto dto)
    {
        var cart = await GetOrCreateCartAsync(customerId);

        if (dto.Items != null && dto.Items.Count != 0)
        {
            var productIds = dto.Items.Select(x => x.ProductId).Distinct().ToList();
            var validProducts = (await _productRepository.GetByIdsAsync(productIds))
                .Where(p => p.Active)
                .ToDictionary(p => p.Id);

            foreach (var syncItem in dto.Items)
            {
                if (!validProducts.ContainsKey(syncItem.ProductId) || syncItem.Quantity <= 0)
                {
                    continue;
                }

                var existingItem = await _cartRepository.GetCartItemByProductAsync(cart.Id, syncItem.ProductId);
                if (existingItem != null)
                {
                    // Merge số lượng từ client vào server
                    existingItem.Quantity += syncItem.Quantity;
                    await _cartRepository.UpdateCartItemAsync(existingItem);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = syncItem.ProductId,
                        Quantity = syncItem.Quantity,
                    };
                    var createdItem = await _cartRepository.AddCartItemAsync(newItem);

                    if (syncItem.AddOnProductIds != null && syncItem.AddOnProductIds.Count != 0)
                    {
                        await AddOrUpdateAddOnsAsync(createdItem.Id, syncItem.AddOnProductIds);
                    }
                }
            }
        }

        return await BuildCartDtoAsync(cart);
    }

    private async Task<Cart> GetOrCreateCartAsync(int customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null)
        {
            cart = await _cartRepository.CreateCartAsync(customerId);
        }
        return cart;
    }

    private async Task AddOrUpdateAddOnsAsync(int cartItemId, IEnumerable<int> addOnProductIds)
    {
        var validAddOns = await _productRepository.GetByIdsAsync(addOnProductIds);
        var toAdd = validAddOns
            .Where(p => p.Active)
            .Select(p => new CartItemAddOn
            {
                CartItemId = cartItemId,
                AddOnProductId = p.Id,
                Quantity = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();

        if (toAdd.Count != 0)
        {
            await _cartRepository.AddCartItemAddOnsAsync(toAdd);
        }
    }

    private async Task<CartDto> BuildCartDtoAsync(Cart cart)
    {
        var items = await _cartRepository.GetCartItemsAsync(cart.Id);
        if (items.Count == 0)
        {
            return new CartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                Items = new List<CartItemDto>(),
                TotalAmount = 0m,
                TotalItems = 0
            };
        }

        var itemIds = items.Select(i => i.Id).ToList();
        var productIds = items.Where(i => i.ProductId.HasValue).Select(i => i.ProductId!.Value).Distinct().ToList();

        var addOns = await _cartRepository.GetAddOnsByCartItemIdsAsync(itemIds);
        var addOnProductIds = addOns.Select(a => a.AddOnProductId).Distinct().ToList();

        var allProductIds = productIds.Concat(addOnProductIds).Distinct().ToList();
        var products = (await _productRepository.GetByIdsAsync(allProductIds)).ToDictionary(p => p.Id);

        var now = DateTime.UtcNow;
        var promotions = await _productRepository.GetActivePromotionsAsync(productIds, now);

        var cartItemDtos = new List<CartItemDto>();
        decimal totalAmount = 0m;
        int totalItems = 0;

        foreach (var item in items)
        {
            if (!item.ProductId.HasValue || !products.TryGetValue(item.ProductId.Value, out var product))
            {
                continue;
            }

            promotions.TryGetValue(product.Id, out var promo);

            decimal unitPrice = product.BasePrice;
            decimal? discountPercent = null;

            if (promo != null && promo.Active && promo.StartAt <= now && promo.EndAt >= now)
            {
                discountPercent = promo.DiscountPercent;
                unitPrice = product.BasePrice * (1 - promo.DiscountPercent / 100);
            }

            var itemAddOns = addOns.Where(a => a.CartItemId == item.Id).ToList();
            var addOnDtos = new List<CartItemAddOnDto>();

            foreach (var addOn in itemAddOns)
            {
                if (products.TryGetValue(addOn.AddOnProductId, out var addOnProd))
                {
                    var addOnDto = new CartItemAddOnDto
                    {
                        Id = addOn.Id,
                        CartItemId = item.Id,
                        AddOnProductId = addOn.AddOnProductId,
                        Name = addOnProd.Name,
                        SKU = addOnProd.SKU,
                        Image = addOnProd.ImageUrl,
                        UnitPrice = addOnProd.BasePrice,
                        Quantity = addOn.Quantity
                    };
                    addOnDtos.Add(addOnDto);
                    totalAmount += addOnDto.Subtotal;
                }
            }

            var itemDto = new CartItemDto
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = product.Id,
                ComboId = item.ComboId,
                Name = product.Name,
                SKU = product.SKU,
                Image = product.ImageUrl,
                BasePrice = product.BasePrice,
                UnitPrice = unitPrice,
                DiscountPercent = discountPercent,
                Quantity = item.Quantity,
                AddOns = addOnDtos
            };

            totalAmount += itemDto.Subtotal;
            totalItems += item.Quantity;
            cartItemDtos.Add(itemDto);
        }

        return new CartDto
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            Items = cartItemDtos,
            TotalAmount = totalAmount,
            TotalItems = totalItems
        };
    }
}
