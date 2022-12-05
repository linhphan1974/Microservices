using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;

namespace BookOnline.Borrowing.Api.Models
{
    public class Member : Entity, IRoot
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public int Status { get; set; }


        public Member(string memberId, string memberName)
        {
            MemberName = memberName;
            Status = (int)MemberStatus.Unlocked;
            MemberId = memberId;
        }

        public void VerifyMember(int borrowId)
        {
            AddDomainEvent(new MemberVerifyDomainEvent(this, borrowId, Status));
        }
    }
}
