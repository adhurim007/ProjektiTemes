using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using StockManagement.Domain.Entities;

namespace StockManagement.Infrastructure
{
    public class StockManagementDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public StockManagementDbContext(DbContextOptions<StockManagementDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .HasColumnType("datetime");
            });
        }
    }
}
