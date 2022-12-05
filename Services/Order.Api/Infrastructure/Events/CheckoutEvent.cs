using BookOnline.Ordering.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public record CheckoutEvent : ApplicationEvent
    {
        public BookBasket Basket { get; set; }
        protected CheckoutEvent(BookBasket bookBasket)
        {
            Basket = bookBasket;
        }
    }
}
