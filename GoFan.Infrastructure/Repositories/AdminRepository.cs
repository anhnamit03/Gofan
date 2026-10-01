using GoFan.Application.Interfaces.Repositories;
using GoFan.Domain.Admins;
using GoFan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoFan.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        return await _context.Admins.FindAsync(id);
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        var lower = email.ToLower();
        return await _context.Admins.FirstOrDefaultAsync(a => a.Email.ToLower() == lower);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var lower = email.ToLower();
        return await _context.Admins.AnyAsync(a => a.Email.ToLower() == lower);
    }

    public async Task<Admin> AddAsync(Admin admin)
    {
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();
        return admin;
    }

    public async Task<bool> UpdateAsync(Admin admin)
    {
        _context.Admins.Update(admin);
        await _context.SaveChangesAsync();
        return true;
    }
}
