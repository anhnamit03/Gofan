namespace GoFan.Domain.Orders;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? ComboId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? SKU { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
} 