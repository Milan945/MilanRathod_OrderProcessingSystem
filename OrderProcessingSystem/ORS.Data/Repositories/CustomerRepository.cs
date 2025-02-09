using Microsoft.EntityFrameworkCore;
using ORS.Data.Models;

namespace ORS.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task SaveChangesAsync();
        Task<bool> HasUnfulfilledOrdersAsync(int customerId);
    }
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ORSDbContext _context;

        public CustomerRepository(ORSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUnfulfilledOrdersAsync(int customerId)
        {
            return await _context.Orders
                .AnyAsync(order => order.CustomerId == customerId && !order.IsFulfilled);
        }
    }
}
