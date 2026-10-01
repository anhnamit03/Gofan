using System.Security.Claims;
using GoFan.Application.DTOs.Auth;
using GoFan.Application.DTOs.Common;
using GoFan.Application.Exceptions;
using GoFan.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFan.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [HttpPost("customer/register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
    {
        var result = await _authService.RegisterCustomerAsync(dto);
        return StatusCode(201, ApiResponse<AuthResponseDto>.Ok(result, "Đăng ký tài khoản thành công", 201));
    }

    [HttpPost("customer/login")]
    public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginDto dto)
    {
        var result = await _authService.CustomerLoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Đăng nhập thành công"));
    }

    [HttpPost("admin/login")]
    public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto dto)
    {
        var result = await _authService.AdminLoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Đăng nhập quản trị thành công"));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "Customer";

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedException("Phiên đăng nhập không hợp lệ.");
        }

        var profile = await _authService.GetProfileAsync(userId, role);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "Customer";

        var updatedProfile = await _authService.UpdateProfileAsync(userId, role, dto);
        return Ok(ApiResponse<UserProfileDto>.Ok(updatedProfile, "Cập nhật thông tin profile thành công"));
    }

    [Authorize]
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("Vui lòng chọn file ảnh để tải lên.");
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            throw new BadRequestException("Dung lượng file không được vượt quá 5MB.");
        }

        var userId = GetCurrentUserId();
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "Customer";
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        using var stream = file.OpenReadStream();
        var avatarUrl = await _authService.UploadAvatarAsync(userId, role, stream, file.FileName, webRoot);

        return Ok(ApiResponse<object>.Ok(new { avatarUrl }, "Cập nhật ảnh đại diện thành công"));
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = GetCurrentUserId();
        var addresses = await _authService.GetCustomerAddressesAsync(userId);
        return Ok(ApiResponse<List<CustomerAddressDto>>.Ok(addresses));
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] CreateCustomerAddressDto dto)
    {
        var userId = GetCurrentUserId();
        var address = await _authService.AddCustomerAddressAsync(userId, dto);
        return StatusCode(201, ApiResponse<CustomerAddressDto>.Ok(address, "Thêm địa chỉ mới thành công", 201));
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateCustomerAddressDto dto)
    {
        var userId = GetCurrentUserId();
        var address = await _authService.UpdateCustomerAddressAsync(userId, id, dto);
        return Ok(ApiResponse<CustomerAddressDto>.Ok(address, "Cập nhật địa chỉ thành công"));
    }

    [Authorize(Roles = "Customer")]
    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = GetCurrentUserId();
        await _authService.DeleteCustomerAddressAsync(userId, id);
        return Ok(ApiResponse<bool>.Ok(true, "Xóa địa chỉ thành công"));
    }

    [Authorize(Roles = "Customer")]
    [HttpPatch("addresses/{id}/default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        var userId = GetCurrentUserId();
        await _authService.SetDefaultCustomerAddressAsync(userId, id);
        return Ok(ApiResponse<bool>.Ok(true, "Đặt địa chỉ mặc định thành công"));
    }

    private int GetCurrentUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }
        return userId;
    }
}
