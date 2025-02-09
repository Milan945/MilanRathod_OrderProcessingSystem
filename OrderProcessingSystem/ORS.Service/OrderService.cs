using ORS.Data.Models;
using ORS.Data.Repositories;
using ORS.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORS.Service
{

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>  
        /// Creates a new order after validating business rules.  
        /// </summary>  
        public async Task CreateOrderAsync(Order order)
        {
            // Validate that the customer has no unfulfilled orders  
            var hasUnfulfilledOrders = await _customerRepository.HasUnfulfilledOrdersAsync(order.CustomerId);
            if (hasUnfulfilledOrders)
            {
                throw new InvalidOperationException("Cannot place a new order while a previous order is unfulfilled.");
            }

            // Calculate total price (optional since TotalPrice is already computed)  
            //order.TotalPrice = order.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price);

            // Add and save the order  
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        /// <summary>  
        /// Marks an order as fulfilled.  
        /// </summary>  
        public async Task FulfillOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            order.IsFulfilled = true;
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }
    }
}
