using Microsoft.AspNetCore.Mvc;
using StockManagement.Application.Services;
using StockManagement.Domain.Entities;

namespace StockManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems() => Ok(await _itemService.GetAllItemsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(Item item)
        {
            await _itemService.AddItemAsync(item);
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, Item item)
        {
            if (id != item.Id) return BadRequest();
            await _itemService.UpdateItemAsync(item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItemAsync(id);
            return NoContent();
        }
    }

}
