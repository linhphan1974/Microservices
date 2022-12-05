using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowConfirmed
{
    public class BorrowConfirmedDomainEventHandler : INotificationHandler<BorrowConfirmedDomainEvent>
    {
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly ILogger<BorrowConfirmedDomainEventHandler> _logger;
        private readonly IMemberRepository _repository;

        public BorrowConfirmedDomainEventHandler(IBorrowingApplicationEventService eventLog, 
            ILogger<BorrowConfirmedDomainEventHandler> logger,
            IMemberRepository repository)
        {
            _eventLog = eventLog;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BorrowConfirmedDomainEvent notification, CancellationToken cancellationToken)
        {
            var member = await _repository.GetByIdAsync(notification.Borrow.MemberId.Value);

            if (member is not null)
            {
                BorrowChangeStatusToConfirmedEvent borrowConfirmedEvent = new BorrowChangeStatusToConfirmedEvent(notification.Borrow.Id, member.MemberName, "Your borrow were confirmed");
                await _eventLog.AddAndSaveEventAsync(borrowConfirmedEvent);
                _logger.LogTrace("Sent borrow confirmed notification to client");
            }
         
            _logger.LogTrace("Member does not exist");
        }
    }
}
