using BookOnline.Book.Api.Infrastucture.Events;
using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.EventHandlers
{
    public class BorrowWaitForStockConfirmEventHandler : IEventHandler<BorrowChangeStatusToWaitForStockConfirmEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IBookRepository _repository;
        private readonly ILogger<BorrowWaitForStockConfirmEventHandler> _logger;
        public BorrowWaitForStockConfirmEventHandler(IEventBus eventBus, IBookRepository repository, ILogger<BorrowWaitForStockConfirmEventHandler> logger)
        {
            _eventBus = eventBus;
            _repository = repository;
            _logger = logger;
        }
        public async Task Handler(BorrowChangeStatusToWaitForStockConfirmEvent @event)
        {
            List<ConfirmedBookItem> confirmItems = new List<ConfirmedBookItem>();
            foreach (var item in @event.Items)
            {
                var isAvailable = await _repository.ConfirmedBookAvailable(item.Id);
                ConfirmedBookItem confirmItem = new ConfirmedBookItem(item.Id, isAvailable);
                confirmItems.Add(confirmItem);
            }

            var isRejected = confirmItems.Any(i => !i.IsAvailable);

            if(isRejected)
            {
                BorrowStockRejectEvent rejectedEvent = new BorrowStockRejectEvent(@event.BorrowId, confirmItems);
                _eventBus.Publish(rejectedEvent);
            }
            else
            {
                BorrowStockConfirmedEvent confirmedEvent = new BorrowStockConfirmedEvent(@event.BorrowId);
                _eventBus.Publish(confirmedEvent);
            }

        }
    }
}
