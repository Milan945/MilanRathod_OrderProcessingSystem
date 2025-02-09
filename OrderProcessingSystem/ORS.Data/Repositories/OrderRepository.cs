using Microsoft.EntityFrameworkCore;
using ORS.Data.Contracts;
using ORS.Data.Models;
using Serilog;

namespace ORS.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ORSDbContext _context;

        public OrderRepository(ORSDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving order with ID {OrderId}.", id);
                throw;
            }
        }

        public async Task AddAsync(Order order)
        {
            try
            {
                await _context.Orders.AddAsync(order);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while adding a new order.");
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
    }
}