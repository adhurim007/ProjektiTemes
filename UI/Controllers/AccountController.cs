using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.UI.Models;

namespace OrderManagementSystem.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<IdentityUser> signInManager,
                                 UserManager<IdentityUser> userManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var userWithRoles = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() // Merr vetëm rolin e parë nëse ka shumë role
                });
            }

            return View(userWithRoles);
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "User account is locked.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Login is not allowed.");
                }
                else if (result.RequiresTwoFactor)
                {
                    ModelState.AddModelError("", "Two-factor authentication is required.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email address is required.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with this email.");
                return View();
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["ResetToken"] = resetToken;

            return RedirectToAction("ConfirmResetPassword", new { email = email, token = resetToken });
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                Roles = new List<string> { "Admin", "Business", "Depo" } // Sigurohuni që këtu të jenë rolet reale
            };
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }

                    return RedirectToAction("Index", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Rimbushni listën e roleve në rast gabimi
            model.Roles = new List<string> { "Admin", "Business", "Depo" };
            return View(model);
        }

        [HttpPost]
        [Route("Account/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID e përdoruesit është e pavlefshme!";
                return RedirectToAction("Index");
            }

            // Gjej përdoruesin nga ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Përdoruesi nuk u gjet!";
                return RedirectToAction("Index");
            }

            // Fshij përdoruesin
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Përdoruesi u fshi me sukses!";
            }
            else
            {
                TempData["ErrorMessage"] = "Diçka shkoi keq gjatë fshirjes së përdoruesit.";
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            return RedirectToAction("Index");
        }
    }
}
