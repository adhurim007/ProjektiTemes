using Microsoft.EntityFrameworkCore;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Interfaces;
using Stripe.Climate;

namespace StockManagement.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _itemRepository.GetAllAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _itemRepository.GetByIdAsync(id);
        }

        public async Task AddItemAsync(Item item)
        {
            await _itemRepository.AddAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            await _itemRepository.UpdateAsync(item);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _itemRepository.DeleteAsync(id);
        }

       
        public async Task UpdateStockAsync(int id, int quantity)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Item not found.");

            item.UpdateStock(quantity);
            await _itemRepository.UpdateAsync(item);
        }
        public async Task<bool> IsItemInUseAsync(int itemId)
        {
            return await _itemRepository.IsItemInUseAsync(itemId);
        }
    }
}
