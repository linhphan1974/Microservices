using BookOnline.Borrowing.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Borrowing.Api.Infrastucture.Data.EntityConfigurations
{
    public class BorrowItemEntityConfiguration : IEntityTypeConfiguration<BorrowItem>
    {
        public void Configure(EntityTypeBuilder<BorrowItem> builder)
        {
            builder.ToTable("BorrowItem");
            builder.HasKey(i => i.Id);
            builder.Ignore(i => i.DomainEvents);
            builder.Property(i => i.Id).UseHiLo("borrow_item_seq");

            builder.Property<int>("BorrowId")
                .IsRequired();

            builder.Property(i => i.BookId)
                .IsRequired();

            builder.Property<string>("_title")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property<string>("_pictureUrl")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PictureUrl")
                .IsRequired()
                .HasMaxLength(500);

            //builder.HasOne<Borrow>()
            //    .WithMany()
            //    .HasForeignKey(i => i.BorrowId)
            //    .HasPrincipalKey(c => c.Id)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
