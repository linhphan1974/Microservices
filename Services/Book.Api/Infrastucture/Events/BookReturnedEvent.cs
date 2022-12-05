using RabbitMQEventBus;

namespace Book.Api.Infrastucture.Events
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
