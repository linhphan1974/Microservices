using RabbitMQEventBus;

namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public record BookReturnedEvent : ApplicationEvent
    {
        public int BookId { get; set; }
        public BookReturnedEvent(int id)
        {
            BookId = id;
        }

    }
}
