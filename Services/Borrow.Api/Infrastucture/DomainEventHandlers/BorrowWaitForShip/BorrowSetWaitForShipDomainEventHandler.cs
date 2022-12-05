using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowWaitForShip
{
    public class BorrowSetWaitForShipDomainEventHandler : INotificationHandler<BorrowSetWaitForShipDomainEvent>
    {
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly ILogger<BorrowSetWaitForShipDomainEventHandler> _logger;
        private readonly IMemberRepository _repository;
        public BorrowSetWaitForShipDomainEventHandler(IBorrowingApplicationEventService eventLog, 
            ILogger<BorrowSetWaitForShipDomainEventHandler> logger, 
            IMemberRepository repository)
        {
            _eventLog = eventLog;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BorrowSetWaitForShipDomainEvent notification, CancellationToken cancellationToken)
        {
            var member = await _repository.GetByIdAsync(notification.Borrow.MemberId.Value);

            if (member is not null)
            {
                BorrowChangeStatusToWaitForShipEvent waitForShipEvent = new BorrowChangeStatusToWaitForShipEvent(
                    notification.Borrow.Id,
                    member.MemberName, "Your borrow was confirmed and ready for ship"
                );
                await _eventLog.AddAndSaveEventAsync(waitForShipEvent);

                _logger.LogTrace("Sent borrow ready for ship notification to client");
            }

            _logger.LogTrace("Member does not exist");
        }
    }
}
