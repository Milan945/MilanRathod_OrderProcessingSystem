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
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete if a customer is deleted  

                entity.Property(o => o.IsFulfilled)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.OrderDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()"); // Default value is the current UTC date and time  

                entity.HasIndex(o => new { o.CustomerId, o.IsFulfilled })
                    .HasFilter("[IsFulfilled] = 0") // SQL Server-specific syntax  
                    .IsUnique(); // Ensure only one unfulfilled order per customer  
            });

            // Configure OrderItem Entity  
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => new { oi.OrderId, oi.ProductId }); // Composite key  

                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete if an order is deleted  

                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a product if it's referenced in an order  

                entity.Property(oi => oi.Quantity)
                    .IsRequired();
            });

        }
    }
}
