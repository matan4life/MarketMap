using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarketMap.Data;

namespace MarketMap.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }

    public class ApplicationUser : IdentityUser
    {
        public double Rating { get; set; }

        public List<Comment> Comments { get; set; }
        public List<FavouriteOutlet> Favourites { get; set; }

        public ApplicationUser() : base()
        {
            Comments = new List<Comment>();
            Favourites = new List<FavouriteOutlet>();
        }
    }

    public class FavouriteOutlet
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int OutletId { get; set; }
        public Outlet Outlet { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public double Rating { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int OutletId { get; set; }
        public Outlet Outlet { get; set; }

        public int? ReplyToCommentId { get; set; }
        public Comment ReplyTo
        {
            get
            {
                using (var db = new ApplicationDbContext())
                {
                    return db.Comments.FirstOrDefault(x => x.Id == ReplyToCommentId);
                }
            }
        }

        public Comment() { }
    }

    public class Color
    {
        public string Name { get; set; }
        
        public float  A { get; set; } /* Alpha */
        public byte   R { get; set; } /* Red   */ 
        public byte   G { get; set; } /* Green */
        public byte   B { get; set; } /* Blue  */

        public string CategoryName { get; set; }
        public Category Category { get; set; }

        public Color() { }

        public override string ToString()
        {
            return $"{Name}(R:{R}, G:{G}, B:{B}, A:{A})";
        }
    }

    public class Category
    {
        public string Name { get; set; }

        public string ColorName { get; set; }
        public Color Color { get; set; }

        public List<OutletCategory> OutletCategories { get; set; }

        public Category()
        {
            OutletCategories = new List<OutletCategory>();
        }

        public override string ToString()
        {
            return $"{Name}: {Color.ToString()}";
        }
    }

    public class OutletCategory
    {
        public int Id { get; set; }

        public int OutletId { get; set; }
        public Outlet Outlet { get; set; }

        public string CategoryName { get; set; }
        public Category Category { get; set; }
    }

    public class Point
    {
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public int Order { get; set; }

        public int OutletId { get; set; }
        public Outlet Outlet { get; set; }

        public Point() { }
        public Point(double latitude, double longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }
    }

    [NotMapped]
    public abstract class Building
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Outlet : Building
    {
        public int Id { get; set; }
        public double Rating { get; set; }
        public string WorkingHours { get; set; }

        /* Debug color */
        public Color Color => new Color { A = 1, R = 102, G = 153, B = 255 };

        public List<Point> Points { get; set; }
        public List<OutletCategory> OutletCategories { get; set; }
        public List<Comment> Comments { get; set; }
        public List<FavouriteOutlet> Favourites { get; set; }
        
        public Outlet() : base()
        {
            Points = new List<Point>();
            OutletCategories = new List<OutletCategory>();
            Comments = new List<Comment>();
            Favourites = new List<FavouriteOutlet>();
        }
    }

    [NotMapped]
    public static class ColorCollection
    {
        public static Color Shared { get; set; }
        private static Dictionary<Category, Color> Values { get; set; }

        public static Color GetColor(Category c) =>
            Values.ContainsKey(c) ? Values[c] : throw new KeyNotFoundException();
    }
}