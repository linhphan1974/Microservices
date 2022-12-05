using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowProcessStartedEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }

        public BorrowProcessStartedEvent(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
