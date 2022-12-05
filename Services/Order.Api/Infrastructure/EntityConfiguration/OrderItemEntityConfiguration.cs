using BookOnline.Ordering.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Ordering.Api.Infrastructure.EntityConfiguration
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Order)
                .WithMany()
                .HasForeignKey(i => i.OrderId);
        }
    }
}
