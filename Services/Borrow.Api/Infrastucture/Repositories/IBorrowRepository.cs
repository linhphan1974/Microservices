using BookOnline.Borrowing.Api.Models;

namespace BookOnline.Borrowing.Api.Infrastucture.Repositories
{
    public interface IBorrowRepository : IRepository<Borrow>
    {
        Task<Borrow> GetByIdAsync(int id);
        Borrow Create(Borrow borrow);
        Borrow Update(Borrow borrow);
        void Delete(int id);

    }
}
