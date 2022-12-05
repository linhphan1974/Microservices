using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowShipped
{
    public class BorrowSetToShippedDomainEventHandler : INotificationHandler<BorrowSetToShippedDomainEvent>
    {
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly ILogger<BorrowSetToShippedDomainEventHandler> _logger;
        private readonly IMemberRepository _repository;
        public BorrowSetToShippedDomainEventHandler(IBorrowingApplicationEventService eventLog,
            ILogger<BorrowSetToShippedDomainEventHandler> logger, IMemberRepository repository)
        {
            _eventLog = eventLog;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BorrowSetToShippedDomainEvent notification, CancellationToken cancellationToken)
        {
            var member = await _repository.GetByIdAsync(notification.Borrow.MemberId.Value);

            if (member != null)
            {
                BorrowChangeStatusToShippedEvent shippedEvent = new BorrowChangeStatusToShippedEvent(notification.Borrow.Id, member.MemberName, "Your borrow was shipped!");
                await _eventLog.AddAndSaveEventAsync(shippedEvent);
                _logger.LogTrace("Borrow change status to shipped");
            }

            _logger.LogTrace("Member does not exist");
        }
    }
}
