using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class CreateBorrowCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public Address Address { get; set; }
        public BookBasket Basket { get; set; }
        public int ShipType { get; set; }

        public CreateBorrowCommand(string userId, string username, Address address, BookBasket basket, int shipType)
        {
            UserId = userId;
            Username = username;
            Address = address;
            Basket = basket;
            ShipType = shipType;
        }
    }
}
