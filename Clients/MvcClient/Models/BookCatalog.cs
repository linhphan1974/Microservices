using System.ComponentModel.DataAnnotations;

namespace BookOnline.MvcClient.Models
{
    public class BookCatalog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Catalog Name")]
        public string Name { get; set; }
    }
}
