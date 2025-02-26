using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Infrastructure
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Shto rolet fillestare
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Detajet e përdoruesit fillestar
            var defaultUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            // Fjalëkalimi i përdoruesit fillestar
            string defaultPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(defaultUser.Email) == null)
            {
                var result = await userManager.CreateAsync(defaultUser, defaultPassword);

                if (result.Succeeded)
                {
                    // Shto përdoruesin në rolin "Admin"
                    await userManager.AddToRoleAsync(defaultUser, "Admin");
                }
            }
        }
    }
}