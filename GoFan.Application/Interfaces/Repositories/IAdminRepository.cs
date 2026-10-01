using GoFan.Domain.Admins;

namespace GoFan.Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<Admin?> GetByIdAsync(int id);
    Task<Admin?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Admin> AddAsync(Admin admin);
    Task<bool> UpdateAsync(Admin admin);
}
