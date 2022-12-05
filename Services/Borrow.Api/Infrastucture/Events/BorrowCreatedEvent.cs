using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BorrowCreatedEvent : ApplicationEvent
    {
        public string MemberId { get; set; }

        public BorrowCreatedEvent(string memberId)
        {
            MemberId = memberId;
        }
    }
}
