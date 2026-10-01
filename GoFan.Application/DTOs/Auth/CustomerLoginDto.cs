using System.ComponentModel.DataAnnotations;

namespace GoFan.Application.DTOs.Auth;

public class CustomerLoginDto
{
    /// <summary>
    /// Số điện thoại hoặc Email
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Có thể truyền trực tiếp Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Có thể truyền trực tiếp Số điện thoại
    /// </summary>
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = string.Empty;

    public string GetIdentifier()
    {
        if (!string.IsNullOrWhiteSpace(Email)) return Email.Trim();
        if (!string.IsNullOrWhiteSpace(Identifier)) return Identifier.Trim();
        if (!string.IsNullOrWhiteSpace(Phone)) return Phone.Trim();
        return string.Empty;
    }
}
