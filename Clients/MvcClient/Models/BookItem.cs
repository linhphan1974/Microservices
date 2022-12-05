using System.ComponentModel.DataAnnotations;

namespace BookOnline.MvcClient.Models
{
    public class BookItemDto
    {
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required, StringLength(12)]
        public string ISBN { get; set; }

        [Display(Name ="First Published")]
        public DateTime FirstPublished { get; set; }

        public string Version { get; set; }
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
