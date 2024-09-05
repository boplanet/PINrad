using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PINrad.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PINrad.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<RegLogUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Primijeni sve migracije na bazu podataka
            context.Database.Migrate();

            // Kreiranje uloga
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Kreiranje početnog admin korisnika
            if (userManager.Users.All(u => u.Email != "admin@pinrad.com"))
            {
                var adminUser = new RegLogUser
                {
                    UserName = "Admin", // Po želji, može biti isti kao email
                    Email = "admin@pinrad.com",
                    PunoIme = "Administrator",
                    Odjel = "IT",
                    Rola = "Admin",
                    EmailConfirmed = true // Potvrdi email odmah
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Kreiranje testnog korisnika
            if (userManager.Users.All(u => u.Email != "testuser@pinrad.com"))
            {
                var testUser = new RegLogUser
                {
                    UserName = "testuser", // Po želji, može biti isti kao email
                    Email = "testuser@pinrad.com",
                    PunoIme = "Test Korisnik",
                    Odjel = "Marketing",
                    Rola = "User",
                    EmailConfirmed = true // Potvrdi email odmah
                };

                var result = await userManager.CreateAsync(testUser, "Test123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, "User");
                }
            }
        }
    }
}
