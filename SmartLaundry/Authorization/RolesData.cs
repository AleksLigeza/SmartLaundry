using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SmartLaundry.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartLaundry.Data;

namespace SmartLaundry.Authorization
{
    public static class RolesData
    {
        private static readonly string[] Roles = new string[] { "Administrator", "Manager", "Porter", "Occupant" };

        public static async Task SeedRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // DISABLED APLYING MIGRATIONS
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    await dbContext.Database.MigrateAsync();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var role in Roles)
                {
                    if (await roleManager.RoleExistsAsync(role)) continue;
                    await roleManager.CreateAsync(new IdentityRole(role));

                    if (role != "Administrator") continue;
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var user = new ApplicationUser
                    {
                        UserName = configuration["AdminEmail"],
                        Email = configuration["AdminEmail"],
                        Firstname = "Admin",
                        Lastname = "Admin",
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, configuration["AdminPassword"]);
                            
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
                }
            }
        }
    }
}
