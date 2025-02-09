using Microsoft.EntityFrameworkCore;
using ORS.Data.Models;

namespace ORS.Data
{
    public class ORSDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ORSDbContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=OrderProcessingSystem;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.PasswordHash).IsRequired().HasMaxLength(256);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.HasOne(o => o.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => new { oi.OrderId, oi.ProductId });
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId);

                entity.HasOne(oi => oi.Product)
                      .WithMany()
                      .HasForeignKey(oi => oi.ProductId);

                entity.Property(oi => oi.Quantity).IsRequired();
            });
        }
    }
}
