using System.ComponentModel.DataAnnotations;

namespace GoFan.Application.DTOs.Auth;

public class RegisterCustomerDto
{
    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FullName { get; set; }
}
