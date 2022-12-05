using BookOnline.Borrowing.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Borrowing.Api.Infrastucture.Data.EntityConfigurations
{
    public class BorrowEntityConfiguration : IEntityTypeConfiguration<Borrow>
    {
        public void Configure(EntityTypeBuilder<Borrow> builder)
        {
            builder.ToTable("Borrow");
            builder.HasKey(b => b.Id);
            builder.Ignore(b => b.DomainEvents);
            builder.Property(i => i.Id).UseHiLo("borrow_seq");

            builder.Property<DateTime>("_borrowDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BorrowDate")
                .IsRequired();

            builder.Property<int>("_borrowStatus")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BorrowStatus")
                .IsRequired();

            builder.Property<DateTime>("_pickupDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PickupDate");

            builder.Property<DateTime>("_returnDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ReturnDate");

            builder.Property<string>("_description")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Description");

            builder.Property<int>("_shipType")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ShipType");

            builder.Property<int?>(i => i.MemberId);

            builder.OwnsOne(o => o.Address, a =>
            {
                a.Property<int>("BorrowId")
                .UseHiLo("borrow_seq");
                a.WithOwner();

            });

            var navigation = builder.Metadata.FindNavigation(nameof(Borrow.Items));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<Member>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(k => k.MemberId);


        }
    }
}
