namespace GoFan.Domain.Carts;

public class Cart
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}