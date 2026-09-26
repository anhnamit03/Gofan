using GoFan.Domain.Products;

namespace GoFan.Domain.Reviews;

public class ReviewMedia
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public MediaType MediaType { get; set; }

    public required string Url { get; set; }

    public DateTime CreatedAt { get; set; }
}