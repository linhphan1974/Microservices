using BookOnline.Aggregator.Models;
using GrpcBorrow;

namespace BookOnline.Aggregator.Services
{
    public interface IBorrowService
    {
        Task<BorrowDto> CreateDraftBorrow(BasketDto basket);
    }
}
