using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookOnline.Borrowing.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string MidleName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PostalCode { get; set; }
    }
}
