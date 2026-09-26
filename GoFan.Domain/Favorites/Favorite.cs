namespace GoFan.Domain.Favorites;

public class Favorite
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public DateTime CreatedAt { get; set; }
}