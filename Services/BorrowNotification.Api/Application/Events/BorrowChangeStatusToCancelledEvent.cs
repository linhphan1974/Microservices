using RabbitMQEventBus;

namespace BookOnline.BorrowNotification.Api.Application.Events
{
    public record BorrowChangeStatusToCancelledEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }
        public string MemberName { get; set; }
        public string Status { get; set; }

        public BorrowChangeStatusToCancelledEvent(int borrowId, string memberName, string status)
        {
            MemberName = memberName;
            BorrowId = borrowId;
            Status = status;
        }
    }
}
