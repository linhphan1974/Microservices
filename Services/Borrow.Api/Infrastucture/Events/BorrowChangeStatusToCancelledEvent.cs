using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowChangeStatusToCancelledEvent : ApplicationEvent
    {
        public string Status { get; set; }
        public string MemberName { get; set; }
        public int BorrowId { get; set; }

        public BorrowChangeStatusToCancelledEvent(int borrowId, string memberName, string status)
        {
            MemberName = memberName;
            BorrowId = borrowId;
            Status = status;
        }
    }
}
