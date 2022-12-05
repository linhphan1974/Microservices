using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowChangeStatusToWaitForShipEvent : ApplicationEvent
    {
        public int BorrowId { get; set; }
        public string MemberName { get; set; }
        public string Status { get; set; }
        public BorrowChangeStatusToWaitForShipEvent(int borrowId, string memberName, string status)
        {
            BorrowId = borrowId;
            MemberName = memberName;
            Status = status;
        }
    }
}
