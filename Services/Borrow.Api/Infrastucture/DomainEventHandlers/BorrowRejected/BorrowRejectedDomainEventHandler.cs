using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers.BorrowRejected
{
    public class BorrowRejectedDomainEventHandler : INotificationHandler<BorrowRejectedDomainEvent>
    {
        private readonly ILogger<BorrowRejectedDomainEventHandler> _logger;
        private readonly IMemberRepository _memberRepository;
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly IMediator _mediator;
        public BorrowRejectedDomainEventHandler(ILogger<BorrowRejectedDomainEventHandler> logger,
            IMediator mediator,
            IMemberRepository memberRepository,
            IBorrowingApplicationEventService eventLog)
        {
            _logger = logger;
            _mediator = mediator;
            _memberRepository = memberRepository;
            _eventLog = eventLog;
        }

        public async Task Handle(BorrowRejectedDomainEvent notification, CancellationToken cancellationToken)
        {
            var member = await _memberRepository.GetByIdAsync(notification.Borrow.MemberId.Value);

            BorrowChangeStatusToRejectedEvent borrowRejectedEvent = new BorrowChangeStatusToRejectedEvent(notification.Borrow.Id, member.MemberName, "Your borrow were rejected");
            await _eventLog.AddAndSaveEventAsync(borrowRejectedEvent);
            _logger.LogTrace("Sent borrow rejected notification to client");
        }
    }
}
