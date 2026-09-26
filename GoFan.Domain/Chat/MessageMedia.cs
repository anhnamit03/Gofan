using GoFan.Domain.Products;

namespace GoFan.Domain.Chat;

public class MessageMedia
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public MediaType MediaType { get; set; }

    public required string Url { get; set; }

    public DateTime CreatedAt { get; set; }
}