using Microsoft.EntityFrameworkCore;
using ScanNShopWebApi.Models;

namespace ScanNShopWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User"); // Erzwingt die Verwendung der richtigen Tabelle
        }

    }
}
