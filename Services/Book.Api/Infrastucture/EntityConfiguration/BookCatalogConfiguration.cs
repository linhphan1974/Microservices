using BookOnline.Book.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Book.Api.Infrastucture.EntityConfiguration
{

    public class BookCatalogConfiguration : IEntityTypeConfiguration<BookCatalog>
    {
        public void Configure(EntityTypeBuilder<BookCatalog> builder)
        {
            builder.ToTable("BookCatalog");
            builder.HasKey(t => t.Id);

            builder.Property(opt => opt.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(opt => opt.Description)
                .HasMaxLength(500);
        }
    }
}
