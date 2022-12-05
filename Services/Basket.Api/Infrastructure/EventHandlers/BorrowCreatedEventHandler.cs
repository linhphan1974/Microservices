using BookOnline.Basket.Api.Infrastructure.Data;
using BookOnline.Basket.Api.Infrastructure.Events;
using RabbitMQEventBus;

namespace BookOnline.Basket.Api.Infrastructure.EventHandlers
{
    public class BorrowCreatedEventHandler : IEventHandler<BorrowCreatedEvent>
    {
        private readonly IBasketRepository _basketRepository;
        public BorrowCreatedEventHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task Handler(BorrowCreatedEvent @event)
        {
            await _basketRepository.DeleteBasketAsync(@event.MemberId);

            // Todo
            // Call SignalR hub to clear cart from client
        }
    }
}
