using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLaundry.Data;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry
{
    public static class RolesData
    {
        private static readonly string[] Roles = new string[] { "Administrator", "Manager", "Porter", "Occupant" };

        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // DISABLED APLYING MIGRATIONS
                //var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                //if (dbContext.Database.GetPendingMigrations().Any())
                //{
                //    await dbContext.Database.MigrateAsync();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var role in Roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));

                        if (role == "Administrator")
                        {
                            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                            var user = new ApplicationUser
                            {
                                UserName = "admin@admin.admin",
                                Email = "admin@admin.admin",
                                Firstname = "Admin",
                                Lastname = "Admin",
                                EmailConfirmed = true
                            };

                            await userManager.CreateAsync(user, "admin123");
                            
                            await userManager.AddToRoleAsync(user, "Administrator");
                        }
                    }
                }
                //}
            }
        }
    }
}
