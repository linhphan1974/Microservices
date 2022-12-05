using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.Events
{
    public record BorrowStockConfirmedEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }

        public BorrowStockConfirmedEvent(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
