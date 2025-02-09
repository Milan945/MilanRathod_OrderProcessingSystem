using ORS.Data.Models;
using ORS.Service.Dtos;

namespace ORS.Service.Contracts
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CustomerOrdersDto order);
        Task<Order?> GetOrderByIdAsync(int id);
        Task FulfillOrderAsync(int orderId);
    }
}
