namespace GoFan.Domain.Inventory;

public class StockAdjustment
{
    public int Id { get; set; }

    public int InventoryId { get; set; }

    public int AdminId { get; set; }

    public int QuantityChange { get; set; }

    public required string Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}