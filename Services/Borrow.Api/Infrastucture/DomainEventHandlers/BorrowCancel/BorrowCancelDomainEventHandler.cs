using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEventHandlers
{
    public class BorrowCancelDomainEventHandler : INotificationHandler<BorrowCancelDomainEvent>
    {
        private readonly IBorrowRepository _repository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoggerFactory _logger;
        private readonly IBorrowingApplicationEventService _eventLog;

        public BorrowCancelDomainEventHandler(IBorrowRepository repository,
            ILoggerFactory logger, 
            IBorrowingApplicationEventService eventLog, 
            IMemberRepository memberRepository)
        {
            _repository = repository;
            _logger = logger;
            _eventLog = eventLog;
            _memberRepository = memberRepository;
        }

        public async Task Handle(BorrowCancelDomainEvent notification, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(notification.Borrow.Id);
            var member = await _memberRepository.GetByIdAsync(notification.Borrow.MemberId.Value);
            
            BorrowCancelEvent borrowCancelEvent = new BorrowCancelEvent(borrow.Items.Select(i=>i.BookId).ToList());
            await _eventLog.AddAndSaveEventAsync(borrowCancelEvent);

            BorrowChangeStatusToCancelledEvent borrowChangeCancelEvent = new BorrowChangeStatusToCancelledEvent(borrow.Id, member.MemberName, "Your borrow were cancelled");
            await _eventLog.AddAndSaveEventAsync(borrowChangeCancelEvent);

            _logger.CreateLogger<BorrowCancelDomainEventHandler>().LogTrace("Borrow cancelled");
        }
    }
}
