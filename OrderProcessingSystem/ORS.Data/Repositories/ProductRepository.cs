using ORS.Data.Models;

namespace ORS.Data.Repositories
{
    public interface IProductRepository
    {
        Product? GetById(int id);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly ORSDbContext _context;

        public ProductRepository(ORSDbContext context)
        {
            _context = context;
        }

        public Product? GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
    }
}
