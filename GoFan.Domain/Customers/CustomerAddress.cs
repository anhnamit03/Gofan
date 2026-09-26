namespace GoFan.Domain.Customers;

public class CustomerAddress
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public required string Province { get; set; }

    public required string District { get; set; }

    public required string Ward { get; set; }

    public required string DetailedAddress { get; set; }

    public string? Note { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}