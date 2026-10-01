using GoFan.Application.Interfaces.Repositories;
using GoFan.Domain.Customers;
using GoFan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoFan.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == email.ToLower());
    }

    public async Task<Customer?> GetByPhoneOrEmailAsync(string identifier)
    {
        var lower = identifier.ToLower();
        return await _context.Customers.FirstOrDefaultAsync(c =>
            c.Phone == identifier || (c.Email != null && c.Email.ToLower() == lower));
    }

    public async Task<bool> ExistsByPhoneAsync(string phone)
    {
        return await _context.Customers.AnyAsync(c => c.Phone == phone);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var lower = email.ToLower();
        return await _context.Customers.AnyAsync(c => c.Email != null && c.Email.ToLower() == lower);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CustomerAddress>> GetAddressesByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerAddresses
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<CustomerAddress?> GetAddressByIdAsync(int addressId, int customerId)
    {
        return await _context.CustomerAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.CustomerId == customerId);
    }

    public async Task<CustomerAddress> AddAddressAsync(CustomerAddress address)
    {
        if (address.IsDefault)
        {
            var existingDefaults = await _context.CustomerAddresses
                .Where(a => a.CustomerId == address.CustomerId && a.IsDefault)
                .ToListAsync();
            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
            }
        }
        else
        {
            var count = await _context.CustomerAddresses.CountAsync(a => a.CustomerId == address.CustomerId);
            if (count == 0)
            {
                address.IsDefault = true;
            }
        }

        await _context.CustomerAddresses.AddAsync(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<bool> UpdateAddressAsync(CustomerAddress address)
    {
        if (address.IsDefault)
        {
            var existingDefaults = await _context.CustomerAddresses
                .Where(a => a.CustomerId == address.CustomerId && a.Id != address.Id && a.IsDefault)
                .ToListAsync();
            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
            }
        }

        _context.CustomerAddresses.Update(address);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAddressAsync(int addressId, int customerId)
    {
        var address = await GetAddressByIdAsync(addressId, customerId);
        if (address == null) return false;

        _context.CustomerAddresses.Remove(address);
        await _context.SaveChangesAsync();

        if (address.IsDefault)
        {
            var next = await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();
            if (next != null)
            {
                next.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        return true;
    }

    public async Task<bool> SetDefaultAddressAsync(int addressId, int customerId)
    {
        var address = await GetAddressByIdAsync(addressId, customerId);
        if (address == null) return false;

        var allAddresses = await _context.CustomerAddresses
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();

        foreach (var item in allAddresses)
        {
            item.IsDefault = (item.Id == addressId);
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
