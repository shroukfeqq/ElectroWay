using ElctroWay.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ElctroWay.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            const string adminEmail = "admin@elctroway.com";
            const string adminPassword = "Admin@123";

            // Create Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>("Admin"));
            }

            // Create Admin User
            var admin =
                await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result =
                    await userManager.CreateAsync(
                        user,
                        adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        user,
                        "Admin");
                }
            }
        }
    }
}