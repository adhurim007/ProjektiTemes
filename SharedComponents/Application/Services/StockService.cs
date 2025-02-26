using SharedComponents.Domain.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedComponents.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IItemRepository _itemRepository;

        public StockService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _itemRepository.GetByIdAsync(id);
        }

        public async Task UpdateItemAsync(Item item)
        {
            await _itemRepository.UpdateAsync(item);
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _itemRepository.GetAllAsync();
        }

        public async Task ReduceStockAsync(int itemId, int quantity)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null) throw new Exception($"Item with ID {itemId} not found.");

            item.StockQuantity -= quantity;
            await _itemRepository.UpdateAsync(item);
        }
         
        Task<bool> IStockService.AddStock(Item item)
        {
            throw new NotImplementedException();
        }

        Task<bool> IStockService.RemoveStock(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
