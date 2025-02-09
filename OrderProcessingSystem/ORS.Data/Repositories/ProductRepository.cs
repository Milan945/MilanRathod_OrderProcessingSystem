using Microsoft.EntityFrameworkCore;
using ORS.Data.Contracts;
using ORS.Data.Models;
using Serilog;

namespace ORS.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ORSDbContext _context;

        public ProductRepository(ORSDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving product with ID {ProductId}.", id);
                throw;
            }
        }
    }
}