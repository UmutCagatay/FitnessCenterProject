using Microsoft.AspNetCore.Identity;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roller = { "Admin", "Member" };

            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            var adminEmail = "g201210029@sakarya.edu.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var yeniAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Yönetici",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(yeniAdmin, "sau");

                await userManager.AddToRoleAsync(yeniAdmin, "Admin");
            }
        }
    }
}