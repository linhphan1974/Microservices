using BookOnline.Basket.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Basket.Api.Infrastructure.Events
{
    public record CheckoutBorrowEvent : ApplicationEvent
    {
        public int ShipType { get; set; }
        public BookBasket Basket { get; set; }
        public ApplicationUser User { get; set; }
        public CheckoutBorrowEvent(int shipType, BookBasket bookBasket, ApplicationUser user)
        {
            Basket = bookBasket;
            ShipType = shipType;
            User = user;
        }
    }
}
