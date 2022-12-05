namespace BookOnline.Book.Api.Models
{
    public class BookItem : Entity
    {
        public string ISBN { get; set; }

        public DateTime FirstPublish { get; set; }

        public string Version { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public BookCatalog Catalog { get; set; }
        public int CatalogId { get; set; }

        public BookType BookType { get; set; }
        public int BookTypeId { get; set; }

        public string Publisher { get; set; }

        public string Author { get; set; }

        public int Status { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }
    }
}
