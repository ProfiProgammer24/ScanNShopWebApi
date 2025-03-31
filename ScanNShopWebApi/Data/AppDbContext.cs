using Microsoft.EntityFrameworkCore;
using ScanNShopWebApi.Models;

namespace ScanNShopWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Product> Products { get; set; } // ✅ HINZUGEFÜGT
        public DbSet<RelatListUser> RelatListUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<List>().ToTable("List");
            modelBuilder.Entity<Product>().ToTable("Product"); // ✅ HINZUGEFÜGT

            modelBuilder.Entity<RelatListUser>()
    .HasKey(r => new { r.Relat_UserId, r.Relat_ListId });

     


        }
    }
}
