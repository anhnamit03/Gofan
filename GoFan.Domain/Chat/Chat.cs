namespace GoFan.Domain.Chat;

public class Conversation
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}