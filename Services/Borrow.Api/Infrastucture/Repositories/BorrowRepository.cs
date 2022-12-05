using BookOnline.Borrowing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookOnline.Borrowing.Api.Infrastucture.Repositories
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly BorrowingDBContext _dbContext;

        public BorrowRepository(BorrowingDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _dbContext; }
        }

        public Borrow Create(Borrow borrow)
        {
           return _dbContext.Borrows.Add(borrow).Entity;
        }

        public void Delete(int id)
        {
            var existBorrow = _dbContext.Borrows.FirstOrDefault(b => b.Id == id);
            _dbContext.Borrows.Remove(existBorrow);
        }

        public async Task<Borrow> GetByIdAsync(int id)
        {
            var borrow = await _dbContext.Borrows.Include(i => i.Items)
                .Include(a => a.Address)
                .FirstOrDefaultAsync(b => b.Id == id);

            if(borrow == null)
            {
                borrow = _dbContext.Borrows.Local.FirstOrDefault(b => b.Id == id);
            }
            if(borrow is not null)
            {
                await _dbContext.Entry(borrow).Collection(b=> b.Items).LoadAsync();
            }

            return borrow;
        }

        public Borrow Update(Borrow borrow)
        {
            return _dbContext.Borrows.Update(borrow).Entity;
        }
    }
}
