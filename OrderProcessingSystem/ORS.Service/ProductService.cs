using ORS.Data.Contracts;
using ORS.Data.Models;
using ORS.Service.Contracts;
using Serilog;

namespace ORS.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving product with ID {ProductId}.", id);
                throw;
            }
        }
    }
}