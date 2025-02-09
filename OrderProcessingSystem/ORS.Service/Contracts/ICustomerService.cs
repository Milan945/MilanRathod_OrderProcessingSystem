using ORS.Data.Models;

namespace ORS.Service.Contracts
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
    }
}
