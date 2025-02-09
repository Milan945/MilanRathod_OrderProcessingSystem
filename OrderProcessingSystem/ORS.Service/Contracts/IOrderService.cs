using ORS.Data.Models;

namespace ORS.Service.Contracts
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int id);
        Task FulfillOrderAsync(int orderId);
    }
}
