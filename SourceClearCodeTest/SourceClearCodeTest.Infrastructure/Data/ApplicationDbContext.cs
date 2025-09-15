using Microsoft.EntityFrameworkCore;
using SourceClearCodeTest.Core.Entities;

namespace SourceClearCodeTest.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, Quantity = 100 },
                new Product { Id = 2, Name = "Product 2", Price = 20.50m, Quantity = 50 },
                new Product { Id = 3, Name = "Product 3", Price = 15.75m, Quantity = 75 }
            );
        }
    }
}