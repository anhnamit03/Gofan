namespace GoFan.Domain.Reviews;

public class Review
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Rating { get; set; }

    public string? Content { get; set; }

    public bool IsHidden { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
}