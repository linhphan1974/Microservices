using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.CommandEventHandlers.BorrowReject
{
    public class UpdateMemberDomainEventHandler : INotificationHandler<MemberVerifyDomainEvent>
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMemberRepository _memberRepository;
        private readonly IBorrowingApplicationEventService _eventLog;
        private readonly BorrowingDBContext _context;
        public UpdateMemberDomainEventHandler(IBorrowRepository borrowRepository, 
            ILoggerFactory loggerFactory,
            IMemberRepository memberRepository,
            IBorrowingApplicationEventService eventLog,
            BorrowingDBContext context)
        {
            _borrowRepository = borrowRepository;
            _loggerFactory = loggerFactory;
            _memberRepository = memberRepository;
            _eventLog = eventLog;
            _context = context;
        }

        public async Task Handle(MemberVerifyDomainEvent notification, CancellationToken cancellationToken)
        {
            var borrow = await _borrowRepository.GetByIdAsync(notification.BorrowId);

            if (notification.MemberStatus == (int)MemberStatus.Unlocked)
            {
                borrow.SetMemberId(notification.Member.Id);
            }
            else
            {
                _borrowRepository.Delete(borrow.Id);

                var domainEntities = _context.ChangeTracker
                    .Entries<Entity>()
                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

                domainEntities.ToList()
                    .ForEach(entity => entity.Entity.ClearDomainEvents());
                
                await _eventLog.DeleteCurrentPendingEvent();
            }
        }
    }
}
