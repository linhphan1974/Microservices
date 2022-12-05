using System.ComponentModel.DataAnnotations;

namespace BookOnline.MvcClient.Models
{
    public class BookType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="Book Type")]
        public string Name { get; set; }
    }
}
