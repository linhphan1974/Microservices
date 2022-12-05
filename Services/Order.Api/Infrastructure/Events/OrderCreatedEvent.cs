using RabbitMQEventBus;

namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public record OrderCreatedEvent : ApplicationEvent
    {
        public string MemberId { get; set; }

        public OrderCreatedEvent(string memberId)
        {
            MemberId = memberId;
        }
    }
}
