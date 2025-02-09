using ORS.Data.Models;
using ORS.Data.Repositories;
using ORS.Service.Contracts;
using ORS.Service.Dtos;
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
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        /// <summary>  
        /// Creates a new order after validating business rules.  
        /// </summary>  

        public async Task CreateOrderAsync(CustomerOrdersDto customerOrdersDto)
        {
            // Validate customer existence  
            var customerExists = await _customerRepository.GetByIdAsync(customerOrdersDto.CustomerId);
            if (customerExists == null)
            {
                throw new InvalidOperationException($"Customer with ID {customerOrdersDto.CustomerId} does not exist.");
            }

            // Validate that the customer has no unfulfilled orders  
            var hasUnfulfilledOrders = await _customerRepository.HasUnfulfilledOrdersAsync(customerOrdersDto.CustomerId);
            if (hasUnfulfilledOrders)
            {
                throw new InvalidOperationException("Cannot place a new order while a previous order is unfulfilled.");
            }

            if (customerOrdersDto.Orders.Any(o => o.Quantity == 0))
            {
                throw new InvalidOperationException("Cannot place a new order with 0 quantity.");
            }

            // Create an Order object  
            var order = new Order
            {
                CustomerId = customerOrdersDto.CustomerId,
                OrderItems = new List<OrderItem>()
            };

            // Process each OrdersDto and add to OrderItems  
            foreach (var orderDto in customerOrdersDto.Orders)
            {
                var product = await _productRepository.GetByIdAsync(orderDto.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {orderDto.ProductId} does not exist.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = orderDto.ProductId,
                    Quantity = orderDto.Quantity,
                    Product = product
                };

                order.OrderItems.Add(orderItem);
            }

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
