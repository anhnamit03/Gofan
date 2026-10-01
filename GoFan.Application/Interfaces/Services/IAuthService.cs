using GoFan.Application.DTOs.Auth;

namespace GoFan.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);
    Task<AuthResponseDto> CustomerLoginAsync(CustomerLoginDto dto);
    Task<AuthResponseDto> AdminLoginAsync(AdminLoginDto dto);
    Task<UserProfileDto> GetProfileAsync(int userId, string role);
    Task<UserProfileDto> UpdateProfileAsync(int userId, string role, UpdateProfileDto dto);
    Task<List<CustomerAddressDto>> GetCustomerAddressesAsync(int customerId);
    Task<CustomerAddressDto> AddCustomerAddressAsync(int customerId, CreateCustomerAddressDto dto);
    Task<CustomerAddressDto> UpdateCustomerAddressAsync(int customerId, int addressId, UpdateCustomerAddressDto dto);
    Task<bool> DeleteCustomerAddressAsync(int customerId, int addressId);
    Task<bool> SetDefaultCustomerAddressAsync(int customerId, int addressId);
    Task<string> UploadAvatarAsync(int userId, string role, Stream fileStream, string fileName, string webRootPath);
}
