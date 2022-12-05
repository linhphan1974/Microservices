using RabbitMQEventBus;

namespace Book.Api.Infrastucture.Events
{
    public record BookWaitForPickupEvent : ApplicationEvent
    {
        public int BookId { get; set; }

        public BookWaitForPickupEvent(int bookId)
        {
            BookId = bookId;
        }
    }
}
