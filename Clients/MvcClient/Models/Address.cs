using System.ComponentModel.DataAnnotations;

namespace BookOnline.MvcClient.Models
{
    public class Address
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Display(Name ="Postal code")]
        public string PostalCode { get; set; }
        [Display(Name = "Zipcode"), Required]
        public string ZipCode { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Email { get; set; }
        [Display(Name = "Phone number"), Required]
        public string PhoneNumber { get; set; }
    }
}
