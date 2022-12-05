using RabbitMQEventBus;

namespace BookOnline.Basket.Api.Infrastructure.Events
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
