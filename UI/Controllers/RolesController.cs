using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementSystem.UI.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index() => View(_roleManager.Roles);

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
