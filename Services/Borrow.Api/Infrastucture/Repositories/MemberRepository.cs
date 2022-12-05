using BookOnline.Borrowing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookOnline.Borrowing.Api.Infrastucture.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly BorrowingDBContext _dbContext;

        public MemberRepository(BorrowingDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _dbContext; }
        }

        public void Add(Member member)
        {
            _dbContext.Members.Add(member);
        }

        public async Task<Member> GetByIdAsync(int id)
        {
            return await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Member> GetByMemberIdAsync(string memberId)
        {
            return await _dbContext.Members.FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public void Update(Member member)
        {
            _dbContext.Update(member);
        }
    }
}
