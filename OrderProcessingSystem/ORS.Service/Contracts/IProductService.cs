using ORS.Data.Models;

namespace ORS.Service.Contracts
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
    }
}
