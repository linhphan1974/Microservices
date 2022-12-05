using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class MemberRejectedDomainEvent : INotification
    {
        public string MemberId { get; set; }

        public MemberRejectedDomainEvent(string memberId)
        {
            MemberId = memberId;
        }
    }
}
