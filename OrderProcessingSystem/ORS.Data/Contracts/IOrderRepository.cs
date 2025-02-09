using ORS.Data.Models;

namespace ORS.Data.Contracts
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}