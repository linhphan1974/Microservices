using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.CommandEventHandlers
{
    public class BorrorwWaitForVerifyMemberDomainEventHandler : INotificationHandler<BorrowCreateDomainEvent>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ILoggerFactory _logger;

        public BorrorwWaitForVerifyMemberDomainEventHandler(IMemberRepository memberRepository, ILoggerFactory logger)
        {
            _memberRepository = memberRepository;
            _logger = logger;
        }

        public async Task Handle(BorrowCreateDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var member = await _memberRepository.GetByMemberIdAsync(notification.UserId);
                var memberExists = member is not null;

                if (member is null)
                {
                    member = new Member(notification.UserId, notification.UserName);
                }
                member.VerifyMember(notification.Borrow.Id);

                if (!memberExists)
                    _memberRepository.Add(member);

                await _memberRepository.UnitOfWork.SaveEntitiesAsync();
                _logger.CreateLogger<BorrorwWaitForVerifyMemberDomainEventHandler>().LogTrace("Update member data");
            }
            catch(Exception ex)
            {
                _logger.CreateLogger<BorrorwWaitForVerifyMemberDomainEventHandler>().LogError(ex, "Update member data fail");
            }
        }
    }
}
