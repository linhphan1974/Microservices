using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowWaitForPickUp
{
    public class BorrowSetWaitForPickupDomainEventHandler : INotificationHandler<BorrowSetWaitForPickupDomainEvent>
    {
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly ILogger<BorrowSetWaitForPickupDomainEventHandler> _logger;
        private readonly IMemberRepository _repository;
        public BorrowSetWaitForPickupDomainEventHandler(IBorrowingApplicationEventService eventLog,
            ILogger<BorrowSetWaitForPickupDomainEventHandler> logger,
            IMemberRepository repository)
        {
            _eventLog = eventLog;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BorrowSetWaitForPickupDomainEvent notification, CancellationToken cancellationToken)
        {
            var member = await _repository.GetByIdAsync(notification.Borrow.MemberId.Value);

            if(member is not null)
            {
                BorrowChangeStatusToWaitForPickupEvent waitForPickupEvent = new BorrowChangeStatusToWaitForPickupEvent(
                    notification.Borrow.Id,
                    member.MemberName, "Your borrow was confirmed and ready for pickup"
                );
                await _eventLog.AddAndSaveEventAsync(waitForPickupEvent);

                _logger.LogTrace("Sent borrow ready for pickup notification to client");
            }

            _logger.LogTrace("Member does not exist");
        }
    }
}
