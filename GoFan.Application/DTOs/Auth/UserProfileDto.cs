namespace GoFan.Application.DTOs.Auth;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CustomerAddressDto> Addresses { get; set; } = new();
}
