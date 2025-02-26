using StockManagement.Domain.Entities; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedComponents.Domain.Interfaces
{
    public interface IStockService
    {
        Task<Item> GetItemByIdAsync(int id);
        Task UpdateItemAsync(Item item);
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task ReduceStockAsync(int itemId, int quantity);
        Task<bool> AddStock(Item item); // Metoda për të shtuar stok
        Task<bool> RemoveStock(int itemId); // Metoda për të hequr stok
    }

}
