using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookOnline.IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string LastName { get; set; }
        public string MidleName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public string PostalCode { get; set; }
    }
}
