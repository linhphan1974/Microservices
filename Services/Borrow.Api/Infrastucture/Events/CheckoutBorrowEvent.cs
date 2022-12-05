using BookOnline.Borrowing.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record CheckoutBorrowEvent : ApplicationEvent
    {
        public BookBasket Basket { get; set; }
        public int ShipType { get; set; }
        public ApplicationUser User { get; set; }
        public CheckoutBorrowEvent(int shipType, BookBasket basket, ApplicationUser user)
        {
            ShipType = shipType;
            Basket = basket;
            User = user;
        }
    }
}
