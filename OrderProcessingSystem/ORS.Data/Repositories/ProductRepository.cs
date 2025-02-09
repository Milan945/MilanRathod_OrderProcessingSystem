using Microsoft.EntityFrameworkCore;
using ORS.Data.Models;

namespace ORS.Data.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly ORSDbContext _context;

        public ProductRepository(ORSDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
