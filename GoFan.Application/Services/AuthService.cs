using GoFan.Application.DTOs.Auth;
using GoFan.Application.Exceptions;
using GoFan.Application.Interfaces.Authentication;
using GoFan.Application.Interfaces.Repositories;
using GoFan.Application.Interfaces.Services;
using GoFan.Domain.Customers;

namespace GoFan.Application.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        ICustomerRepository customerRepository,
        IAdminRepository adminRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _customerRepository = customerRepository;
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto)
    {
        var phoneClean = dto.Phone.Trim();
        if (await _customerRepository.ExistsByPhoneAsync(phoneClean))
        {
            throw new BadRequestException("Số điện thoại này đã được đăng ký.");
        }

        string? emailClean = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLowerInvariant();
        if (emailClean != null && await _customerRepository.ExistsByEmailAsync(emailClean))
        {
            throw new BadRequestException("Email này đã được sử dụng.");
        }

        var customer = new Customer
        {
            Phone = phoneClean,
            Email = emailClean,
            FullName = dto.FullName?.Trim(),
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _customerRepository.AddAsync(customer);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
            created.Id,
            "Customer",
            created.Phone,
            created.FullName);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = created.Id,
                Role = "Customer",
                FullName = created.FullName,
                Email = created.Email,
                Phone = created.Phone,
                AvatarUrl = created.AvatarUrl,
                Active = created.Active,
                CreatedAt = created.CreatedAt
            }
        };
    }

    public async Task<AuthResponseDto> CustomerLoginAsync(CustomerLoginDto dto)
    {
        var identifier = dto.GetIdentifier();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new BadRequestException("Vui lòng cung cấp Email hoặc Số điện thoại để đăng nhập.");
        }

        var customer = await _customerRepository.GetByPhoneOrEmailAsync(identifier);

        if (customer == null || !_passwordHasher.VerifyPassword(dto.Password, customer.PasswordHash))
        {
            throw new UnauthorizedException("Số điện thoại/Email hoặc mật khẩu không chính xác.");
        }

        if (!customer.Active)
        {
            throw new ForbiddenException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ bộ phận hỗ trợ.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
            customer.Id,
            "Customer",
            customer.Phone,
            customer.FullName);

        var addresses = await _customerRepository.GetAddressesByCustomerIdAsync(customer.Id);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = customer.Id,
                Role = "Customer",
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                AvatarUrl = customer.AvatarUrl,
                Active = customer.Active,
                CreatedAt = customer.CreatedAt,
                Addresses = addresses.Select(MapToAddressDto).ToList()
            }
        };
    }

    public async Task<AuthResponseDto> AdminLoginAsync(AdminLoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var admin = await _adminRepository.GetByEmailAsync(email);

        if (admin == null || !_passwordHasher.VerifyPassword(dto.Password, admin.PasswordHash))
        {
            throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");
        }

        if (!admin.Active)
        {
            throw new ForbiddenException("Tài khoản quản trị đã bị vô hiệu hóa.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
            admin.Id,
            "Admin",
            admin.Email,
            admin.FullName);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = admin.Id,
                Role = "Admin",
                FullName = admin.FullName,
                Email = admin.Email,
                Phone = null,
                AvatarUrl = admin.AvatarUrl,
                Active = admin.Active,
                CreatedAt = admin.CreatedAt
            }
        };
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId, string role)
    {
        if (role == "Admin")
        {
            var admin = await _adminRepository.GetByIdAsync(userId);
            if (admin == null)
            {
                throw new NotFoundException("Tài khoản quản trị không tồn tại.");
            }

            return new UserProfileDto
            {
                Id = admin.Id,
                Role = "Admin",
                FullName = admin.FullName,
                Email = admin.Email,
                Phone = null,
                AvatarUrl = admin.AvatarUrl,
                Active = admin.Active,
                CreatedAt = admin.CreatedAt
            };
        }
        else
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                throw new NotFoundException("Tài khoản khách hàng không tồn tại.");
            }

            var addresses = await _customerRepository.GetAddressesByCustomerIdAsync(userId);

            return new UserProfileDto
            {
                Id = customer.Id,
                Role = "Customer",
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                AvatarUrl = customer.AvatarUrl,
                Active = customer.Active,
                CreatedAt = customer.CreatedAt,
                Addresses = addresses.Select(MapToAddressDto).ToList()
            };
        }
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, string role, UpdateProfileDto dto)
    {
        if (role == "Admin")
        {
            var admin = await _adminRepository.GetByIdAsync(userId);
            if (admin == null)
            {
                throw new NotFoundException("Tài khoản quản trị không tồn tại.");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                admin.FullName = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
            {
                var trimmed = dto.AvatarUrl.Trim();
                if (trimmed.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                {
                    trimmed = await SaveBase64AvatarAsync(admin.Id, trimmed);
                }
                admin.AvatarUrl = trimmed;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var cleanEmail = dto.Email.Trim().ToLowerInvariant();
                if (cleanEmail != admin.Email.ToLowerInvariant())
                {
                    if (await _adminRepository.ExistsByEmailAsync(cleanEmail))
                    {
                        throw new BadRequestException("Email này đã được sử dụng bởi quản trị viên khác.");
                    }
                    admin.Email = cleanEmail;
                }
            }

            admin.UpdatedAt = DateTime.UtcNow;
            await _adminRepository.UpdateAsync(admin);

            return new UserProfileDto
            {
                Id = admin.Id,
                Role = "Admin",
                FullName = admin.FullName,
                Email = admin.Email,
                Phone = null,
                AvatarUrl = admin.AvatarUrl,
                Active = admin.Active,
                CreatedAt = admin.CreatedAt,
                Addresses = new List<CustomerAddressDto>()
            };
        }
        else
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                throw new NotFoundException("Tài khoản khách hàng không tồn tại.");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                customer.FullName = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
            {
                var trimmed = dto.AvatarUrl.Trim();
                if (trimmed.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                {
                    trimmed = await SaveBase64AvatarAsync(customer.Id, trimmed);
                }
                customer.AvatarUrl = trimmed;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var cleanEmail = dto.Email.Trim().ToLowerInvariant();
                if (cleanEmail != customer.Email?.ToLowerInvariant())
                {
                    if (await _customerRepository.ExistsByEmailAsync(cleanEmail))
                    {
                        throw new BadRequestException("Email này đã được sử dụng bởi khách hàng khác.");
                    }
                    customer.Email = cleanEmail;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var cleanPhone = dto.Phone.Trim();
                if (cleanPhone != customer.Phone)
                {
                    if (await _customerRepository.ExistsByPhoneAsync(cleanPhone))
                    {
                        throw new BadRequestException("Số điện thoại này đã được sử dụng bởi khách hàng khác.");
                    }
                    customer.Phone = cleanPhone;
                }
            }

            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer);

            // 1. Thêm địa chỉ mới nếu có yêu cầu
            if (dto.NewAddress != null)
            {
                var address = new CustomerAddress
                {
                    CustomerId = customer.Id,
                    Province = dto.NewAddress.Province.Trim(),
                    District = dto.NewAddress.District.Trim(),
                    Ward = dto.NewAddress.Ward.Trim(),
                    DetailedAddress = dto.NewAddress.DetailedAddress.Trim(),
                    Note = dto.NewAddress.Note?.Trim(),
                    IsDefault = dto.NewAddress.IsDefault,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _customerRepository.AddAddressAsync(address);
            }

            // 2. Cập nhật thông tin địa chỉ sẵn có nếu có truyền
            if (dto.UpdateAddress != null)
            {
                var addrToUpdate = await _customerRepository.GetAddressByIdAsync(dto.UpdateAddress.Id, customer.Id);
                if (addrToUpdate != null)
                {
                    addrToUpdate.Province = dto.UpdateAddress.Province.Trim();
                    addrToUpdate.District = dto.UpdateAddress.District.Trim();
                    addrToUpdate.Ward = dto.UpdateAddress.Ward.Trim();
                    addrToUpdate.DetailedAddress = dto.UpdateAddress.DetailedAddress.Trim();
                    addrToUpdate.Note = dto.UpdateAddress.Note?.Trim();
                    addrToUpdate.IsDefault = dto.UpdateAddress.IsDefault;
                    addrToUpdate.UpdatedAt = DateTime.UtcNow;

                    await _customerRepository.UpdateAddressAsync(addrToUpdate);
                }
            }

            // 3. Đặt địa chỉ mặc định theo DefaultAddressId nếu được truyền
            if (dto.DefaultAddressId.HasValue && dto.DefaultAddressId.Value > 0)
            {
                var address = await _customerRepository.GetAddressByIdAsync(dto.DefaultAddressId.Value, customer.Id);
                if (address == null)
                {
                    throw new NotFoundException($"Không tìm thấy địa chỉ có ID {dto.DefaultAddressId.Value} của bạn để đặt làm mặc định.");
                }

                await _customerRepository.SetDefaultAddressAsync(dto.DefaultAddressId.Value, customer.Id);
            }

            var addresses = await _customerRepository.GetAddressesByCustomerIdAsync(customer.Id);

            return new UserProfileDto
            {
                Id = customer.Id,
                Role = "Customer",
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                AvatarUrl = customer.AvatarUrl,
                Active = customer.Active,
                CreatedAt = customer.CreatedAt,
                Addresses = addresses.Select(MapToAddressDto).ToList()
            };
        }
    }

    public async Task<List<CustomerAddressDto>> GetCustomerAddressesAsync(int customerId)
    {
        var addresses = await _customerRepository.GetAddressesByCustomerIdAsync(customerId);
        return addresses.Select(MapToAddressDto).ToList();
    }

    public async Task<CustomerAddressDto> AddCustomerAddressAsync(int customerId, CreateCustomerAddressDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new NotFoundException("Tài khoản khách hàng không tồn tại.");
        }

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            Province = dto.Province.Trim(),
            District = dto.District.Trim(),
            Ward = dto.Ward.Trim(),
            DetailedAddress = dto.DetailedAddress.Trim(),
            Note = dto.Note?.Trim(),
            IsDefault = dto.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _customerRepository.AddAddressAsync(address);
        return MapToAddressDto(created);
    }

    public async Task<CustomerAddressDto> UpdateCustomerAddressAsync(int customerId, int addressId, UpdateCustomerAddressDto dto)
    {
        var address = await _customerRepository.GetAddressByIdAsync(addressId, customerId);
        if (address == null)
        {
            throw new NotFoundException("Không tìm thấy địa chỉ cần cập nhật.");
        }

        address.Province = dto.Province.Trim();
        address.District = dto.District.Trim();
        address.Ward = dto.Ward.Trim();
        address.DetailedAddress = dto.DetailedAddress.Trim();
        address.Note = dto.Note?.Trim();
        address.IsDefault = dto.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAddressAsync(address);
        return MapToAddressDto(address);
    }

    public async Task<bool> DeleteCustomerAddressAsync(int customerId, int addressId)
    {
        var success = await _customerRepository.DeleteAddressAsync(addressId, customerId);
        if (!success)
        {
            throw new NotFoundException("Không tìm thấy địa chỉ cần xóa.");
        }
        return true;
    }

    public async Task<bool> SetDefaultCustomerAddressAsync(int customerId, int addressId)
    {
        var success = await _customerRepository.SetDefaultAddressAsync(addressId, customerId);
        if (!success)
        {
            throw new NotFoundException("Không tìm thấy địa chỉ để đặt làm mặc định.");
        }
        return true;
    }

    private static CustomerAddressDto MapToAddressDto(CustomerAddress a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        Province = a.Province,
        District = a.District,
        Ward = a.Ward,
        DetailedAddress = a.DetailedAddress,
        Note = a.Note,
        IsDefault = a.IsDefault,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };

    public async Task<string> UploadAvatarAsync(int userId, string role, Stream fileStream, string fileName, string webRootPath)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowedExts.Contains(ext))
        {
            throw new BadRequestException("Định dạng file ảnh không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP, GIF.");
        }

        var uploadsDir = Path.Combine(webRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        var newFileName = $"avatar_{userId}_{DateTime.UtcNow.Ticks}{ext}";
        var fullPath = Path.Combine(uploadsDir, newFileName);

        using (var dest = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(dest);
        }

        var relativeUrl = $"/uploads/avatars/{newFileName}";

        if (role == "Admin")
        {
            var admin = await _adminRepository.GetByIdAsync(userId);
            if (admin != null)
            {
                admin.AvatarUrl = relativeUrl;
                admin.UpdatedAt = DateTime.UtcNow;
                await _adminRepository.UpdateAsync(admin);
            }
        }
        else
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer != null)
            {
                customer.AvatarUrl = relativeUrl;
                customer.UpdatedAt = DateTime.UtcNow;
                await _customerRepository.UpdateAsync(customer);
            }
        }

        return relativeUrl;
    }

    private static async Task<string> SaveBase64AvatarAsync(int userId, string dataUrl)
    {
        try
        {
            var parts = dataUrl.Split(',');
            if (parts.Length < 2) return dataUrl;

            var meta = parts[0];
            var base64 = parts[1];

            var ext = ".png";
            if (meta.Contains("jpeg") || meta.Contains("jpg")) ext = ".jpg";
            else if (meta.Contains("webp")) ext = ".webp";
            else if (meta.Contains("gif")) ext = ".gif";

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var uploadsDir = Path.Combine(baseDir, "wwwroot", "uploads", "avatars");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = $"avatar_{userId}_{DateTime.UtcNow.Ticks}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            var bytes = Convert.FromBase64String(base64);
            await File.WriteAllBytesAsync(filePath, bytes);
            return $"/uploads/avatars/{fileName}";
        }
        catch
        {
            return dataUrl;
        }
    }
}
