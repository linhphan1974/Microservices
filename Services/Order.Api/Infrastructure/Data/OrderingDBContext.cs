using BookOnline.Ordering.Api.Infrastructure.EntityConfiguration;
using BookOnline.Ordering.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BookOnline.Ordering.Api.Infrastructure.Data
{
    public class OrderingDBContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderingDBContext(DbContextOptions<OrderingDBContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new OrderEntityConfiguration());
            builder.ApplyConfiguration(new OrderItemEntityConfiguration());        }
    }
}
