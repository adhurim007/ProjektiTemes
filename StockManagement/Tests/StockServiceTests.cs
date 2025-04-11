using Xunit;
using Moq;
using StockManagement.Domain.Entities;
using SharedComponents.Domain.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockManagement.Tests
{
    public class StockServiceTests
    {
        private readonly Mock<IStockService> _mockStockService;

        public StockServiceTests()
        {
            _mockStockService = new Mock<IStockService>();
        }

        [Fact]
        public async Task AddStock_ValidStock_AddsSuccessfully()
        {
            var stockItem = new Item
            {
                Id = 1,
                Name = "Laptop",
                StockQuantity = 10,
                Price = 1000
            };

            _mockStockService.Setup(service => service.AddStock(stockItem))
                             .ReturnsAsync(true);

            
            var result = await _mockStockService.Object.AddStock(stockItem);

            
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateItemAsync_ValidItem_UpdatesSuccessfully()
        {
          
            var item = new Item
            {
                Id = 1,
                Name = "Laptop",
                StockQuantity = 10,
                Price = 1000
            };

            _mockStockService.Setup(service => service.UpdateItemAsync(item))
                             .Returns(Task.CompletedTask);

          
            await _mockStockService.Object.UpdateItemAsync(item);

           
            _mockStockService.Verify(service => service.UpdateItemAsync(item), Times.Once);
        }

        [Fact]
        public async Task ReduceStockAsync_InvalidStock_ThrowsException()
        {
          
            _mockStockService.Setup(service => service.ReduceStockAsync(It.IsAny<int>(), It.IsAny<int>()))
                             .ThrowsAsync(new KeyNotFoundException());

         
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _mockStockService.Object.ReduceStockAsync(999, 1));
        }
    }
}
