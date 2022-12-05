using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowStockRejectEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }
        public List<ConfirmedBookItem> ConfirmedBookItems { get; set; }

        public BorrowStockRejectEvent(int borrowId, List<ConfirmedBookItem> confirmedBookItems)
        {
            BorrowId = borrowId;
            ConfirmedBookItems = confirmedBookItems;
        }
    }

    public class ConfirmedBookItem
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }

        public ConfirmedBookItem(int id, bool isAvailable)
        {
            id = Id;
            IsAvailable = isAvailable;
        }
    }

}
