using ORS.Data.Models;

namespace ORS.Data.Contracts
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> HasUnfulfilledOrdersAsync(int customerId);
        Task SaveChangesAsync();
    }
}