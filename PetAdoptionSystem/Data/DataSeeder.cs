using Microsoft.AspNetCore.Identity;
using PetAdoptionSystem.Models;

namespace PetAdoptionSystem.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminEmail = "admin@petadoption.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                var result = await userManager.CreateAsync(user, "Admin123!"); // choose a secure password

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        public static async Task SeedPets(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            if (!context.Pets.Any())
            {
                context.Pets.AddRange(
                    new Pet { Name = "Buddy", Species = "Dog", Age = 3, IsAdopted = false },
                    new Pet { Name = "Mittens", Species = "Cat", Age = 2, IsAdopted = false },
                    new Pet { Name = "Charlie", Species = "Parrot", Age = 1, IsAdopted = false }
                );

                await context.SaveChangesAsync();
            }
        }

    }
}
