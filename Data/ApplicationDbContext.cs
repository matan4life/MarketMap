using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MarketMap.Models;

namespace MarketMap.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Point> Points { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<OutletCategory> OutletCategories { get; set; }
        public DbSet<FavouriteOutlet> FavouriteOutlets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasKey(u => u.Id);

            builder.Entity<Point>()
                .HasKey(p => new { p.Latitude, p.Longtitude });

            builder.Entity<Outlet>()
                .HasKey(o => o.Id);

            builder.Entity<Outlet>()
                .HasMany(x => x.Points)
                .WithOne(x => x.Outlet);

            builder.Entity<Color>()
                .HasKey(c => c.Name);

            builder.Entity<Color>()
                .HasOne(c => c.Category)
                .WithOne(c => c.Color);

            builder.Entity<Category>()
                .HasKey(c => c.Name);

            builder.Entity<Category>()
                .HasOne(c => c.Color)
                .WithOne(c => c.Category);

            builder.Entity<OutletCategory>()
                .HasKey(oc => oc.Id);

            builder.Entity<OutletCategory>()
                .HasOne(oc => oc.Category)
                .WithMany(oc => oc.OutletCategories);

            builder.Entity<OutletCategory>()
                .HasOne(oc => oc.Outlet)
                .WithMany(oc => oc.OutletCategories);

            builder.Entity<OutletCategory>()
                .HasOne(oc => oc.Outlet)
                .WithMany(o => o.OutletCategories)
                .HasForeignKey(oc => oc.OutletId);

            builder.Entity<OutletCategory>()
                .HasOne(oc => oc.Category)
                .WithMany(c => c.OutletCategories)
                .HasForeignKey(oc => oc.CategoryName);

            builder.Entity<FavouriteOutlet>()
                .HasKey(fo => fo.Id);    

            builder.Entity<FavouriteOutlet>()
                .HasOne(fo => fo.User)
                .WithMany(u => u.Favourites);

            builder.Entity<FavouriteOutlet>()
                .HasOne(fo => fo.Outlet)
                .WithMany(o => o.Favourites);

            builder.Entity<Comment>()
                .HasKey(u => u.Id);

            builder.Entity<Comment>()
                .Property(c => c.ReplyToCommentId)
                .IsRequired(false);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments);

            builder.Entity<Comment>()
                .HasOne(c => c.Outlet)
                .WithMany(o => o.Comments);

            builder.Entity<Comment>()
                .HasIndex(x => x.ReplyToCommentId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MarketMap;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
