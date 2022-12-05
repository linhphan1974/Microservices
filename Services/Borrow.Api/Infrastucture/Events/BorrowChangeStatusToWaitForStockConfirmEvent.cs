using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowChangeStatusToWaitForStockConfirmEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }
        public List<BorrowBookItem> Items { get; set; }

        public BorrowChangeStatusToWaitForStockConfirmEvent(int borrowId, List<BorrowBookItem> items)
        {
            Items = items;
            BorrowId = borrowId;
        }
    }

    public class BorrowBookItem
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }

        public BorrowBookItem(int id, bool isAvailable)
        {
            Id = id;
            IsAvailable = isAvailable;
        }
    }
}
