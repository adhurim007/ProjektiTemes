using Xunit;
using Moq;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using SharedComponents.Domain.Interfaces;

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

        //[Fact]
        //public async Task GetAllOrdersAsync_ReturnsOrders()
        //{
        //    // Arrange
        //    var orders = new List<Order>
        //    {
        //        new Order("Business1", new List<OrderItem>
        //        {
        //            new OrderItem("Item1", 2, 10),
        //        }),
        //        new Order("Business2", new List<OrderItem>
        //        {
        //            new OrderItem("Item2", 1, 20),
        //        })
        //    };

        //    _mockOrderService.Setup(service => service.GetAllOrdersAsync())
        //                     .ReturnsAsync(orders);

        //    // Act
        //    var result = await _mockOrderService.Object.GetAllOrdersAsync();

        //    // Assert
        //    Assert.Equal(2, result.Count());
        //    Assert.Equal("Business1", result.First().BusinessId);
        //    Assert.Equal("Business2", result.Last().BusinessId);
        //}

        //[Fact]
        //public async Task ReduceStockAsync_InvalidStock_ThrowsException()
        //{
        //    // Arrange
        //    _mockStockService.Setup(service => service.ReduceStockAsync(It.IsAny<int>(), It.IsAny<int>()))
        //                     .ThrowsAsync(new KeyNotFoundException());

        //    // Act & Assert
        //    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        //        _mockStockService.Object.ReduceStockAsync(999, 1));
        //}

        //[Fact]
        //public async Task CreateOrderAsync_ValidOrder_CreatesSuccessfully()
        //{
        //    // Arrange
        //    var order = new Order("Business1", new List<OrderItem>
        //    {
        //        new OrderItem("Item1", 2, 10),
        //    });

        //    _mockOrderService.Setup(service => service.CreateOrderAsync(order, order.Items))
        //                     .Returns(Task.CompletedTask);

        //    // Act
        //    await _mockOrderService.Object.CreateOrderAsync(order, order.Items);

        //    // Assert
        //    _mockOrderService.Verify(service => service.CreateOrderAsync(order, order.Items), Times.Once);
        //}

        //[Fact]
        //public async Task GetOrderWithItemsAsync_ReturnsOrderWithItems()
        //{
        //    // Arrange
        //    var order = new Order("Business1", new List<OrderItem>
        //    {
        //        new OrderItem("Item1", 2, 10),
        //        new OrderItem("Item2", 3, 15)
        //    });

        //    _mockOrderService.Setup(service => service.GetOrderWithItemsAsync(It.IsAny<int>()))
        //                     .ReturnsAsync(order);

        //    // Act
        //    var result = await _mockOrderService.Object.GetOrderWithItemsAsync(order.Id);

        //    // Assert
        //    Assert.Equal(2, result.Items.Count);
        //    Assert.Equal("Business1", result.BusinessId);
        //}

        [Fact]
        public async Task DeleteOrderAsync_InvalidId_ThrowsException()
        {
            // Arrange
            _mockOrderService.Setup(service => service.DeleteOrderAsync(It.IsAny<int>()))
                             .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _mockOrderService.Object.DeleteOrderAsync(999));
        }

        [Fact]
        public void MarkAsPaid_ValidOrder_UpdatesStatus()
        {
            // Arrange
            var order = new Order("Business1", new List<OrderItem>
            {
                new OrderItem("Item1", 2, 10)
            });

            // Act
            order.MarkAsPaid();

            // Assert
            Assert.Equal(OrderStatus.Paid, order.Status);
        }

        [Fact]
        public void MarkAsPaid_AlreadyPaid_ThrowsException()
        {
            // Arrange
            var order = new Order("Business1", new List<OrderItem>
            {
                new OrderItem("Item1", 2, 10)
            });

            order.MarkAsPaid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.MarkAsPaid());
        }
    }
}
