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
                customer.CreatedAt = DateTime.UtcNow;
                customer.UpdatedAt = DateTime.UtcNow;
                await _context.Customers.AddAsync(customer);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while adding a new customer.");
                throw;
            }
        }

        public async Task UpdateAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customer.Id);

                if (existingCustomer == null)
                {
                    Log.Warning("Customer with ID {CustomerId} not found for update.", customer.Id);
                    throw new InvalidOperationException($"Customer with ID {customer.Id} not found.");
                }

                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.PasswordHash = customer.PasswordHash;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Warning(ex, "A concurrency conflict occurred while updating customer with ID {CustomerId}.", customer.Id);
                throw; 
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while updating customer with ID {CustomerId}.", customer.Id);
                throw;
            }
        }

        // Save changes to the database  
        public async Task SaveChangesAsync()
        {
            try
            {
                // Automatically update UpdatedAt timestamps before saving  
                foreach (var entry in _context.ChangeTracker.Entries<Customer>())
                {
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Warning(ex, "A concurrency conflict occurred while saving changes to the database.");
                throw;
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