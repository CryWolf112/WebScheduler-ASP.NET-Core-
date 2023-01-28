using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WebScheduler.Models;
using WebScheduler.Repositories;

namespace WebScheduler.Database
{
    public class DbInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public DbInitializer(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task Run()
        {
            await roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin",
                NormalizedName = roleManager.NormalizeKey("Admin")
            });

            await roleManager.CreateAsync(new IdentityRole()
            {
                Name = "User",
                NormalizedName = roleManager.NormalizeKey("User")
            });

            User admin = new User()
            {
                UserName = "admin",
                Email = "default@vvg.hr",
                EmailConfirmed = true,
                LockoutEnabled = false,
                DateCreated = DateTime.UtcNow,
            };

            await userManager.CreateAsync(admin, "admin");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
