namespace GoFan.Domain.Notifications;

public class Notification
{
    public int Id { get; set; }

    public int RecipientId { get; set; }

    public NotificationType Type { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public bool IsRead { get; set; }

    public int? ReferenceId { get; set; }

    public DateTime CreatedAt { get; set; }
}