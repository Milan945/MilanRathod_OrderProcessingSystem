using Moq;
using ORS.Data.Contracts;
using ORS.Data.Models;
using ORS.Service;
using ORS.Service.Dtos;
using Xunit;

namespace ORS.Tests
{
    [TestClass]
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _productRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task CreateOrderAsync_ShouldThrowException_WhenCustomerDoesNotExist()
        {
            // Arrange  
            var customerOrdersDto = new CustomerOrdersDto { CustomerId = 1, Orders = new List<OrderDto>() };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync((Customer?)null);

            // Act & Assert  
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _orderService.CreateOrderAsync(customerOrdersDto));
        }

        [TestMethod]
        public async Task CreateOrderAsync_ShouldThrowException_WhenCustomerHasUnfulfilledOrders()
        {
            // Arrange  
            var customerOrdersDto = new CustomerOrdersDto { CustomerId = 1, Orders = new List<OrderDto>() };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(new Customer());
            _customerRepositoryMock.Setup(repo => repo.HasUnfulfilledOrdersAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(true);

            // Act & Assert  
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _orderService.CreateOrderAsync(customerOrdersDto));
        }

        [TestMethod]
        public async Task CreateOrderAsync_ShouldThrowException_WhenOrderHasZeroQuantity()
        {
            // Arrange  
            var customerOrdersDto = new CustomerOrdersDto
            {
                CustomerId = 1,
                Orders = new List<OrderDto> { new OrderDto { ProductId = 1, Quantity = 0 } }
            };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(new Customer());
            _customerRepositoryMock.Setup(repo => repo.HasUnfulfilledOrdersAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(false);

            // Act & Assert  
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _orderService.CreateOrderAsync(customerOrdersDto));
        }

        [TestMethod]
        public async Task CreateOrderAsync_ShouldCalculateTotalPriceCorrectly()
        {
            // Arrange  
            var customerOrdersDto = new CustomerOrdersDto
            {
                CustomerId = 1,
                Orders = new List<OrderDto>
            {
                new OrderDto { ProductId = 1, Quantity = 2 },
                new OrderDto { ProductId = 2, Quantity = 3 }
            }
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(new Customer());
            _customerRepositoryMock.Setup(repo => repo.HasUnfulfilledOrdersAsync(customerOrdersDto.CustomerId))
                .ReturnsAsync(false);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Price = 10 });
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(new Product { Id = 2, Price = 20 });

            _orderRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act  
            await _orderService.CreateOrderAsync(customerOrdersDto);

            // Assert  
            _orderRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Order>(o =>
                o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price) == 80)), Times.Once);
        }

        [TestMethod]
        public async Task FulfillOrderAsync_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange  
            var orderId = 1;
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            // Act & Assert  
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _orderService.FulfillOrderAsync(orderId));
        }

        [TestMethod]
        public async Task FulfillOrderAsync_ShouldMarkOrderAsFulfilled()
        {
            // Arrange  
            var orderId = 1;
            var order = new Order { Id = orderId, IsFulfilled = false };
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act  
            await _orderService.FulfillOrderAsync(orderId);

            // Assert  
            Assert.IsTrue(order.IsFulfilled);
            _orderRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}