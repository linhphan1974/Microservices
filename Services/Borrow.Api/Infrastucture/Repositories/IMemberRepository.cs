using BookOnline.Borrowing.Api.Models;

namespace BookOnline.Borrowing.Api.Infrastucture.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<Member> GetByIdAsync(int id);
        Task<Member> GetByMemberIdAsync(string memberId);
        void Update(Member member);
        void Add(Member member);
    }
}
