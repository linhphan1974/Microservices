using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Models.Dto;
using X.PagedList;

namespace BookOnline.MvcClient.Services
{
    public interface IBorrowService
    {
        Task<IPagedList<Borrow>> GetBorrowsByMemberAsync(string memberId, int pageIndex, int pageSize);
        Task<IPagedList<Borrow>> GetBorrowsByStatusAsync(int status, int pageIndex, int pageSize);
        Task<IPagedList<Borrow>> GetBorrowsByDateAsync(DateTime borrowDate, int pageIndex, int pageSize);
        Task<IPagedList<Borrow>> GetBorrowsAsync(string memberId, int? status, DateTime? borrowDate, int pageIndex, int pageSize);
        Task<IPagedList<Borrow>> GetActiveBorrowsAsync(string memberId);
        Task<Borrow> GetDraftBorrow(string memberId);
        Task<bool> Checkout(Borrow borrow);
        Task<bool> Pickup(int id);
        Task<bool> Cancel(int id);  
        Task<bool> SetWaitForPickup(int id);
        Task<bool> SetWaitForShip(int id);
        Task<bool> Ship(int id);
        Task<bool> Return(int id);
    }
}
