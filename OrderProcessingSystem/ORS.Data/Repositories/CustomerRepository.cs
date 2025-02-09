using Microsoft.EntityFrameworkCore;
using ORS.Data.Contracts;
using ORS.Data.Models;
using Serilog;

namespace ORS.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ORSDbContext _context;

        public CustomerRepository(ORSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            try
            {
                return await _context.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving all customers.");
                throw;
            }
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving customer with ID {CustomerId}.", id);
                throw;
            }
        }

        public async Task AddAsync(Customer customer)
        {
            try
            {
                await _context.Customers.AddAsync(customer);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while adding a new customer.");
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while saving changes to the database.");
                throw;
            }
        }

        public async Task<bool> HasUnfulfilledOrdersAsync(int customerId)
        {
            try
            {
                return await _context.Orders
                    .AnyAsync(order => order.CustomerId == customerId && !order.IsFulfilled);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while checking unfulfilled orders for customer with ID {CustomerId}.", customerId);
                throw;
            }
        }
    }
}