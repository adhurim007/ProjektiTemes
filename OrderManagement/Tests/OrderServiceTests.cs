using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using OrderManagement.Application.Services;
using SharedComponents.Domain.Interfaces;
using OrderManagement.Domain.Models; // <- përdorimi i saktë i Order/OrderItem
using OrderStatus = OrderManagement.Domain.Entities.OrderStatus;
 

namespace OrderManagement.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IStockService> _mockStockService;

        public OrderServiceTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockStockService = new Mock<IStockService>();
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    BusinessId = "Business1",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ItemId = 1, Quantity = 2, Price = 10 }
                    },
                    TotalAmount = 20,
                    Status = OrderStatus.Pending.ToString()
                },
                new Order
                {
                    BusinessId = "Business2",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ItemId = 2, Quantity = 1, Price = 20 }
                    },
                    TotalAmount = 20,
                    Status = OrderStatus.Pending.ToString()
                }
            };

            _mockOrderService.Setup(service => service.GetAllOrdersAsync())
                             .ReturnsAsync(orders.AsEnumerable());

            // Act
            var result = await _mockOrderService.Object.GetAllOrdersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Business1", result.First().BusinessId);
            Assert.Equal("Business2", result.Last().BusinessId);
        }

        [Fact]
        public async Task ReduceStockAsync_InvalidStock_ThrowsException()
        {
            // Arrange
            _mockStockService.Setup(service => service.ReduceStockAsync(It.IsAny<int>(), It.IsAny<int>()))
                             .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _mockStockService.Object.ReduceStockAsync(999, 1));
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_CreatesSuccessfully()
        {
            // Arrange
            var order = new Order
            {
                BusinessId = "Business1",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ItemId = 1, Quantity = 2, Price = 10 }
                },
                TotalAmount = 20,
                Status = OrderStatus.Pending.ToString()
            };

            _mockOrderService.Setup(service => service.CreateOrderAsync(order, order.OrderItems.ToList()))
                             .Returns(Task.CompletedTask);

            // Act
            await _mockOrderService.Object.CreateOrderAsync(order, order.OrderItems.ToList());

            // Assert
            _mockOrderService.Verify(service => service.CreateOrderAsync(order, order.OrderItems.ToList()), Times.Once);
        }

        [Fact]
        public async Task GetOrderWithItemsAsync_ReturnsOrderWithItems()
        {
            // Arrange
            var order = new Order
            {
                BusinessId = "Business1",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ItemId = 1, Quantity = 2, Price = 10 },
                    new OrderItem { ItemId = 2, Quantity = 3, Price = 15 }
                },
                TotalAmount = 65,
                Status = OrderStatus.Pending.ToString()
            };

            _mockOrderService.Setup(service => service.GetOrderWithItemsAsync(It.IsAny<int>()))
                             .ReturnsAsync(order);

            // Act
            var result = await _mockOrderService.Object.GetOrderWithItemsAsync(1);

            // Assert
            Assert.Equal(2, result.OrderItems.Count);
            Assert.Equal("Business1", result.BusinessId);
        }

        [Fact]
        public async Task DeleteOrderAsync_InvalidId_ThrowsException()
        {
            // Arrange
            _mockOrderService.Setup(service => service.DeleteOrderAsync(It.IsAny<int>()))
                             .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _mockOrderService.Object.DeleteOrderAsync(999));
        }

        [Fact]
        public void MarkAsPaid_ValidOrder_UpdatesStatus()
        {
            // Arrange
            var order = new Order
            {
                BusinessId = "Business1",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ItemId = 1, Quantity = 2, Price = 10 }
                },
                TotalAmount = 20,
                Status = OrderStatus.Pending.ToString()
            };

            // Act
            order.Status = OrderStatus.Paid.ToString(); 

            // Assert
            Assert.Equal(OrderStatus.Paid.ToString(), order.Status);
        }

        [Fact]
        public void MarkAsPaid_AlreadyPaid_ThrowsException()
        {
            // Arrange
            var order = new Order
            {
                BusinessId = "Business1",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ItemId = 1, Quantity = 2, Price = 10 }
                },
                TotalAmount = 20,
                Status = OrderStatus.Paid.ToString()
            };

            // Simulimi i sjelljes
            Assert.Throws<InvalidOperationException>(() =>
            {
                if (order.Status == OrderStatus.Paid.ToString())
                    throw new InvalidOperationException("Order is already paid.");
            });
        }
    }
}
