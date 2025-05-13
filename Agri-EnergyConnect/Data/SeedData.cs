using Agri_EnergyConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Agri_EnergyConnect.Data 
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roleNames = { "Farmer", "Employee", "Admin" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create test farmer
            var farmerEmail = "farmer@agriconnect.com";
            if (await userManager.FindByEmailAsync(farmerEmail) == null)
            {
                var farmer = new ApplicationUser
                {
                    UserName = farmerEmail,
                    Email = farmerEmail,
                    EmailConfirmed = true,
                    Name = "Test",
                    Surname = "Farmer"
                };
                await userManager.CreateAsync(farmer, "Farmer@123");
                await userManager.AddToRoleAsync(farmer, "Farmer");
            }

            // Create admin (optional)
            var adminEmail = "admin@agriconnect.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "Admin",
                    Surname = "User"
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}