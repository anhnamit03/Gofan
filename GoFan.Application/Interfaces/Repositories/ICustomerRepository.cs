using GoFan.Domain.Customers;

namespace GoFan.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneOrEmailAsync(string identifier);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Customer> AddAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);

    Task<List<CustomerAddress>> GetAddressesByCustomerIdAsync(int customerId);
    Task<CustomerAddress?> GetAddressByIdAsync(int addressId, int customerId);
    Task<CustomerAddress> AddAddressAsync(CustomerAddress address);
    Task<bool> UpdateAddressAsync(CustomerAddress address);
    Task<bool> DeleteAddressAsync(int addressId, int customerId);
    Task<bool> SetDefaultAddressAsync(int addressId, int customerId);
}
