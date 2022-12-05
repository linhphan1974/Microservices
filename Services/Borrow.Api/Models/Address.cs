namespace BookOnline.Borrowing.Api.Models
{
    public class Address : ValueObject
    {
        public String City { get; private set; }
        public String State { get; private set; }
        public String PostalCode { get; private set; }
        public String ZipCode { get; private set; }
        public String Street { get; private set; }
        public String Email { get; private set; }
        public String PhoneNumber { get; private set; }

        public Address(string city, string state, string postalCode, string zipCode, string street, string email, string phoneNumber)
        {
            City = city;
            State = state;
            PostalCode = postalCode;
            ZipCode = zipCode;
            Street = street;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public Address()
        {

        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return Email;
            yield return PhoneNumber;
            yield return PostalCode;
        }
    }
}
