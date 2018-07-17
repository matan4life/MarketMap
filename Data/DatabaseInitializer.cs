using MarketMap.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MarketMap.Models;
using Microsoft.AspNetCore.Identity;

namespace MarketMap.Data
{
    public class DatabaseInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "marketmap@nure.ua";
            string password = "MarketMap1-";
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("Manager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    //await userManager.AddToRoleAsync(admin, "Manager");
                }
            }
        }
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var db = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!db.Colors.Any())
                {
                    db.Colors.AddRange(new Color[]
                    {
                        new Color { Name = "orange", A = 1, R = 230, G = 111, B = 25 },
                        new Color { Name = "light blue", A = 1, R = 25, G = 189, B = 230 },
                        new Color { Name = "light green", A = 1, R = 25, G = 230, B = 29 },
                        new Color { Name = "purple", A = 1, R = 189, G = 25, B = 230 },
                        new Color { Name = "crimson", A = 1, R = 230, G = 25, B = 87 }
                    });
                }

                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(new Category[] 
                    {
                        new Category
                        {
                            Name = "Plants", Color = db.Colors.Find("light green")
                        },
                        new Category
                        {
                            Name = "Fruit", Color = db.Colors.Find("crimson")
                        },
                        new Category
                        {
                            Name = "Toys", Color = db.Colors.Find("orange")
                        },
                        new Category
                        {
                            Name = "Clothes", Color = db.Colors.Find("purple")
                        },
                        new Category
                        {
                            Name = "Sea Food", Color = db.Colors.Find("light blue")
                        }
                    });
                }

                if (!db.Outlets.Any())
                {
                    //Outlet outlet = new Outlet
                    //{
                    //    Name = "Test Outlet",
                    //    Address = "Test Address",
                    //    Rating = 0,
                    //    Points = new List<Point>
                    //    {
                    //        new Point { Id = 1, Latitude = 1, Longtitude = 1 },
                    //        new Point { Id = 2, Latitude = 2, Longtitude = 2 },
                    //        new Point { Id = 3, Latitude = 3, Longtitude = 3 }
                    //    },
                    //    OutletCategories = new OutletCategory {  }
                    //};
                }

                db.SaveChanges();
            }
        }
    }
}
