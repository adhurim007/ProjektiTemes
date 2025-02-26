using Microsoft.AspNetCore.Mvc; 
using StockManagement.Application.Services;
using StockManagement.Domain.Entities;
using System.Threading.Tasks;

namespace OrderManagementSystem.UI.Controllers
{
    public class StockController : Controller
    {
        private readonly IItemService _itemService;

        public StockController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // Shfaq të gjitha artikujt
        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetAllItemsAsync();
            return View(items);
        }

        // Forma për shtimin e artikullit
        public IActionResult Create()
        {
            return View();
        }

        // Shto artikullin e ri
        [HttpPost]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                await _itemService.AddItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // Forma për editimin e artikullit
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // Përditëso artikullin
        [HttpPost]
        public async Task<IActionResult> Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                if (item.CreatedDate == default(DateTime))
                {
                    item.CreatedDate = DateTime.Now;
                } 
                await _itemService.UpdateItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // Konfirmo fshirjen e artikullit
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // Fshi artikullin
        [HttpPost]
        [Route("Customer/DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kontrollo nëse artikulli është në përdorim
            var isInUse = await _itemService.IsItemInUseAsync(id);
            if (isInUse)
            {
                // Kthe një mesazh gabimi nëse artikulli përdoret në OrderItems
                TempData["ErrorMessage"] = "Artikulli nuk mund të fshihet sepse është përdorur në porosi.";
                return RedirectToAction(nameof(Index));
            }

            // Nëse nuk përdoret, fshi artikullin
            await _itemService.DeleteItemAsync(id);
            TempData["SuccessMessage"] = "Artikulli u fshi me sukses.";
            return RedirectToAction(nameof(Index));
        }

    }
}
