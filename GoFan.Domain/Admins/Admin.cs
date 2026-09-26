namespace GoFan.Domain.Admins;

public class Admin
{
    public int Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}