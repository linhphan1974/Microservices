using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Service;
using MediatR;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowWaitForConfirm
{
    public class BorrowSetWaitForConfirmDomainEventHandler : INotificationHandler<BorrowSetWaitForConfirmDomainEvent>
    {
        private readonly ILogger<BorrowSetWaitForConfirmDomainEventHandler> _logger;
        private readonly IBorrowingApplicationEventService _eventLog;

        public BorrowSetWaitForConfirmDomainEventHandler(ILogger<BorrowSetWaitForConfirmDomainEventHandler> logger, IBorrowingApplicationEventService eventLog)
        {
            _logger = logger;
            _eventLog = eventLog;
        }

        public async Task Handle(BorrowSetWaitForConfirmDomainEvent notification, CancellationToken cancellationToken)
        {
            var items = notification.Borrow.Items.Select(i => new BorrowBookItem(i.BookId, false)).ToList();

            BorrowChangeStatusToWaitForStockConfirmEvent borrowWaitForStockConfirmEvent = new BorrowChangeStatusToWaitForStockConfirmEvent(notification.Borrow.Id, items);
            await _eventLog.AddAndSaveEventAsync(borrowWaitForStockConfirmEvent);
        }
    }
}
