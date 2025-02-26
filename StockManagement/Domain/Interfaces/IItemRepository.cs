using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using StockManagement.Domain.Entities;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace StockManagement.Domain.Interfaces
{
    public interface IItemRepository: IRepository<Item>
    {
        Task<Item> GetByIdAsync(int id);
        Task<IEnumerable<Item>> GetAllAsync();
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(int id);
        Task<bool> IsItemInUseAsync(int itemId);


    }
}
 