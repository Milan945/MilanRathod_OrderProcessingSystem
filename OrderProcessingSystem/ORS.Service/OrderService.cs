using ORS.Data.Contracts;
using ORS.Data.Models;
using ORS.Service.Contracts;
using ORS.Service.Dtos;
using Serilog;

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

        public async Task CreateOrderAsync(CustomerOrdersDto customerOrdersDto)
        {
            try
            {
                var customerExists = await _customerRepository.GetByIdAsync(customerOrdersDto.CustomerId);
                if (customerExists == null)
                {
                    Log.Warning("Customer with ID {CustomerId} does not exist.", customerOrdersDto.CustomerId);
                    throw new InvalidOperationException($"Customer with ID {customerOrdersDto.CustomerId} does not exist.");
                }

                var hasUnfulfilledOrders = await _customerRepository.HasUnfulfilledOrdersAsync(customerOrdersDto.CustomerId);
                if (hasUnfulfilledOrders)
                {
                    Log.Warning("Customer with ID {CustomerId} has unfulfilled orders.", customerOrdersDto.CustomerId);
                    throw new InvalidOperationException("Cannot place a new order while a previous order is unfulfilled.");
                }

                if (customerOrdersDto.Orders.Any(o => o.Quantity == 0))
                {
                    Log.Warning("Customer with ID {CustomerId} attempted to place an order with 0 quantity.", customerOrdersDto.CustomerId);
                    throw new InvalidOperationException("Cannot place a new order with 0 quantity.");
                }

                var order = new Order
                {
                    CustomerId = customerOrdersDto.CustomerId,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var orderDto in customerOrdersDto.Orders)
                {
                    var product = await _productRepository.GetByIdAsync(orderDto.ProductId);
                    if (product == null)
                    {
                        Log.Warning("Product with ID {ProductId} does not exist.", orderDto.ProductId);
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
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while creating a new order for Customer ID {CustomerId}.", customerOrdersDto.CustomerId);
                throw;
            }
        }

        public async Task FulfillOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    Log.Warning("Order with ID {OrderId} not found.", orderId);
                    throw new InvalidOperationException($"Order with ID {orderId} not found.");
                }

                order.IsFulfilled = true;
                await _orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while fulfilling order with ID {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _orderRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while retrieving order with ID {OrderId}.", id);
                throw;
            }
        }
    }
}