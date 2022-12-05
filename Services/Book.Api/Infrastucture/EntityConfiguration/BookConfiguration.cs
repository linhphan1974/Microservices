using BookOnline.Book.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Book.Api.Infrastucture.EntityConfiguration
{
    public class BookConfiguration : IEntityTypeConfiguration<BookItem>
    {
        public void Configure(EntityTypeBuilder<BookItem> builder)
        {
            builder.ToTable("BookItem");
            builder.HasKey(x => x.Id);
            
            builder.Property(opt => opt.Author)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(opt => opt.Description)
                .HasMaxLength(1000);

            builder.Property(opt => opt.ISBN)
                .IsRequired()
                .HasMaxLength(17);

            builder.Property(opt => opt.Publisher)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(opt => opt.Quantity)
                .IsRequired();

            builder.Property(opt => opt.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(opt => opt.Version)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(p=>p.BookType)
                .WithMany()
                .HasForeignKey(ci => ci.BookTypeId);

            builder.HasOne(p=>p.Catalog)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogId);

        }
    }
}
