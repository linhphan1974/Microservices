using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class MemberVerifyDomainEvent : INotification
    {
        public Member Member { get; set; }
        public int BorrowId { get; set; }
        public int MemberStatus { get; set; }

        public MemberVerifyDomainEvent(Member member, int borrowId, int memberStatus)
        {
            Member = member;
            BorrowId = borrowId;
            MemberStatus = memberStatus;
        }
    }
}
