using Xunit;
using Moq;
using StockManagement.Domain.Entities;
using SharedComponents.Domain.Interfaces;

namespace StockManagement.Tests
{
    public class StockServiceTests
    {
        private readonly Mock<IStockService> _mockStockService;

        public StockServiceTests()
        {
            _mockStockService = new Mock<IStockService>();
        }

        //[Fact]
        //public void AddStock_ValidStock_AddsSuccessfully()
        //{
        //    // Arrange
        //    var stock = new Item
        //    {
        //        Id = 1,
        //        Name = "Laptop",
        //        StockQuantity = 10,
        //        Price = 1000
        //    };
        //    _mockStockService.Setup(service => service.AddStock(stock)).Returns(true);

        //    // Act
        //    var result = _mockStockService.Object.AddStock(stock);

        //    // Assert
        //    Assert.True(result);
        //}

        [Fact]
        public async Task UpdateItemAsync_ValidItem_UpdatesSuccessfully()
        {
            // Arrange
            var item = new Item
            {
                Id = 1,
                Name = "Laptop",
                StockQuantity = 10,
                Price = 1000
            };

            _mockStockService.Setup(service => service.UpdateItemAsync(item)).Returns(Task.CompletedTask);

            // Act
            await _mockStockService.Object.UpdateItemAsync(item);

            // Assert
            _mockStockService.Verify(service => service.UpdateItemAsync(item), Times.Once);
        }


        [Fact]
        public async Task RemoveStock_InvalidStock_ThrowsException()
        {
            // Arrange
            _mockStockService.Setup(service => service.ReduceStockAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _mockStockService.Object.ReduceStockAsync(999, 1));
        }

    }
}
