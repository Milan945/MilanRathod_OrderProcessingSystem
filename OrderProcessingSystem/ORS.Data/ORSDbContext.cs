using Microsoft.EntityFrameworkCore;
using ORS.Data.Models;
using Serilog;

namespace ORS.Data
{
    public class ORSDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ORSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=OrderProcessingSystem;Integrated Security=True;TrustServerCertificate=True");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while configuring the database context.");
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<Customer>(entity =>
                {
                    entity.HasKey(c => c.Id);
                    entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                    entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                    entity.Property(c => c.PasswordHash).IsRequired().HasMaxLength(256);
                    entity.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                    entity.Property(c => c.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                    entity.Property(c => c.RowVersion).IsRowVersion();
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
                        .OnDelete(DeleteBehavior.Cascade);
                    entity.Property(o => o.IsFulfilled)
                        .IsRequired()
                        .HasDefaultValue(false);
                    entity.Property(o => o.OrderDate)
                        .IsRequired()
                        .HasDefaultValueSql("GETUTCDATE()");
                    entity.HasIndex(o => new { o.CustomerId, o.IsFulfilled })
                        .HasFilter("[IsFulfilled] = 0")
                        .IsUnique();
                });

                modelBuilder.Entity<OrderItem>(entity =>
                {
                    entity.HasKey(oi => new { oi.OrderId, oi.ProductId });
                    entity.HasOne(oi => oi.Order)
                        .WithMany(o => o.OrderItems)
                        .HasForeignKey(oi => oi.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity.HasOne(oi => oi.Product)
                        .WithMany()
                        .HasForeignKey(oi => oi.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.Property(oi => oi.Quantity)
                        .IsRequired();
                });
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while creating the model.");
                throw;
            }
        }
    }
}