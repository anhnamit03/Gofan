namespace GoFan.Application.DTOs.Auth;

public class UpdateProfileDto
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    /// <summary>
    /// ID của địa chỉ đã có trong sổ địa chỉ mà bạn muốn đặt làm địa chỉ mặc định
    /// </summary>
    public int? DefaultAddressId { get; set; }

    /// <summary>
    /// Thêm một địa chỉ mới trực tiếp trong lần cập nhật profile này (tuỳ chọn)
    /// </summary>
    public CreateCustomerAddressDto? NewAddress { get; set; }

    /// <summary>
    /// Cập nhật thông tin của một địa chỉ đã có trong cùng lần gọi này (tuỳ chọn)
    /// </summary>
    public UpdateCustomerAddressWithIdDto? UpdateAddress { get; set; }
}

public class UpdateCustomerAddressWithIdDto
{
    public int Id { get; set; }
    public required string Province { get; set; }
    public required string District { get; set; }
    public required string Ward { get; set; }
    public required string DetailedAddress { get; set; }
    public string? Note { get; set; }
    public bool IsDefault { get; set; }
}
