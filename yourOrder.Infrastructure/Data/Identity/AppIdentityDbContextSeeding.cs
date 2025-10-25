using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.Infrastructure.Data.Identity
{
    public class AppIdentityDbContextSeeding
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILoggerFactory loggerFactory)
        {
            try {
                if (!await userManager.Users.AnyAsync()) //if (!userManager.Users.Any())
                {
                    // 1. Create roles if they do not exist
                    var roles = new List<IdentityRole>
                    {
                        new IdentityRole("Admin"),
                        new IdentityRole("Customer")
                    };
                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(role);
                    }

                    var adminUser = new AppUser
                    {
                        FirstName ="mahmoud",
                        SecondName ="salah",
                        DisplayName = "mahmoud salah",
                        Email = "mahmoud@gmail.com",
                        UserName = "mahmoud@gmail.com",
                        Addresse = new Address
                        {
                            Street = "123 Admin St",
                            City = "alex",
                            Country = "egypt",
                        }
                    };
                    var customerUser = new AppUser
                    {
                        FirstName ="sara",
                        SecondName ="salah",
                        DisplayName = "sara salah",
                        Email = "sara@gmail.com",
                        UserName = "sara@gmail.com",
                        Addresse = new Address
                        {
                            Street = "123 Admin St",
                            City = "alex",
                            Country = "egypt",
                        }
                    };
                    await userManager.CreateAsync(adminUser, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.CreateAsync(customerUser, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(customerUser, "Customer");

                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<AppIdentityDbContextSeeding>();
                logger.LogError(ex.Message);
            }
            
        }
        
    }
}