using BookOnline.Ordering.Api.Infrastructure.Events;
using RabbitMQEventBus;

namespace BookOnline.Ordering.Api.Infrastructure.EventHandler
{
    public class CheckoutEventHandler : IEventHandler<CheckoutEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        public Task Handler(CheckoutEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
