namespace GoFan.Domain.Chat;

public class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string? Content { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsRecalled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}