using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowCancelEvent : ApplicationEvent
    {
        public List<int> BookIds { get; set; }
        public BorrowCancelEvent(List<int> bookids)
        {
            BookIds = bookids;
        }
    }
}
