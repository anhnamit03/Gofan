namespace GoFan.Domain.Orders;

public class OrderItemAddOn
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public int AddOnProductId { get; set; }

    public string AddOnProductName { get; set; } = null!;

    public string? SKU { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}