namespace GoFan.Domain.Carts;

public class CartItemAddOn
{
    public int Id { get; set; }

    public int CartItemId { get; set; }

    public int AddOnProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}