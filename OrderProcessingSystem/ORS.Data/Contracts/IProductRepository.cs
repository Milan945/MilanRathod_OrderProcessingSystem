using ORS.Data.Models;

namespace ORS.Data.Contracts
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
    }
}
