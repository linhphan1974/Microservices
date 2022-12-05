using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.Events
{
    public record BorrowCancelEvent : ApplicationEvent
    {
        public List<int> BookIds { get; set; }
        public BorrowCancelEvent(List<int> bookIds)
        {
            BookIds = bookIds;
        }
    }
}
