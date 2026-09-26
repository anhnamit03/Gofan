namespace GoFan.Domain.Carts;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int? ProductId { get; set; }

    public int? ComboId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}