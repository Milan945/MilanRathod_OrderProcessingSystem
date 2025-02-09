using ORS.Data.Models;

namespace ORS.Service.Contracts
{
    public interface IProductService
    {
        Product? GetProductById(int id);
    }
}
