using ORS.Data.Contracts;
using ORS.Data.Models;
using ORS.Service.Contracts;
using Serilog;

namespace ORS.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _customerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving all customers.");
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _customerRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving customer with ID {CustomerId}.", id);
                throw;
            }
        }
    }
}