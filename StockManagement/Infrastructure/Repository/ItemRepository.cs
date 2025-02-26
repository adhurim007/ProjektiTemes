using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Interfaces;

namespace StockManagement.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly StockManagementDbContext _context;

        public ItemRepository(StockManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task AddAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }
         
        public async Task UpdateAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsItemInUseAsync(int itemId)
        {
            return await _context.OrderItems.AnyAsync(oi => oi.ItemId == itemId);
        }

    }
}
