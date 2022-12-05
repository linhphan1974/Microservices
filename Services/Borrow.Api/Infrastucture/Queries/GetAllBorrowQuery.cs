
using BookOnline.Borrowing.Api.ViewModels;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetAllBorrowQuery : IRequest<List<BorrowDto>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public GetAllBorrowQuery(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
