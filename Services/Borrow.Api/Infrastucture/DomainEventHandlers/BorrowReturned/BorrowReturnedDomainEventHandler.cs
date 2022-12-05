using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowReturned
{
    public class BorrowReturnedDomainEventHandler : INotificationHandler<BorrowReturnedDomainEvent>
    {
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly IBorrowRepository _repository;
        private readonly ILoggerFactory _logger;

        public BorrowReturnedDomainEventHandler(IBorrowingApplicationEventService eventLog, IBorrowRepository repository, ILoggerFactory logger)
        {
            _eventLog = eventLog;
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(BorrowReturnedDomainEvent notification, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(notification.Borrow.Id);
            var ids = borrow.Items.Select(i => i.BookId).ToList();

            await _eventLog.AddAndSaveEventAsync(new BookChangeQualityAfterReturnEvent(ids));
            _logger.CreateLogger<BorrowReturnedDomainEventHandler>().LogTrace("Send event to update book quality after book returned");
        }
    }
}
