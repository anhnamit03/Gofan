namespace GoFan.Application.DTOs.Auth;

public class CustomerAddressDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string DetailedAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCustomerAddressDto
{
    public required string Province { get; set; }
    public required string District { get; set; }
    public required string Ward { get; set; }
    public required string DetailedAddress { get; set; }
    public string? Note { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateCustomerAddressDto
{
    public required string Province { get; set; }
    public required string District { get; set; }
    public required string Ward { get; set; }
    public required string DetailedAddress { get; set; }
    public string? Note { get; set; }
    public bool IsDefault { get; set; }
}
