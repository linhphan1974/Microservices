using BookOnline.Book.Api.Infrastucture.EntityConfiguration;
using BookOnline.Book.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookOnline.Book.Api.Infrastucture.Data
{
    public class BookDBContext : DbContext
    {
        public DbSet<BookType> BookTypes { get; set; }

        public DbSet<BookCatalog> BookCatalogs { get; set; }

        public DbSet<BookItem> BookItems { get; set; }

        public BookDBContext(DbContextOptions<BookDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookCatalogConfiguration());
            modelBuilder.ApplyConfiguration(new BookTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
        }
    }
}
