namespace GoFan.Application.DTOs.Carts;

public class CartDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int? ProductId { get; set; }
    public int? ComboId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Image { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public List<CartItemAddOnDto> AddOns { get; set; } = new();
}

public class CartItemAddOnDto
{
    public int Id { get; set; }
    public int CartItemId { get; set; }
    public int AddOnProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Image { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}
