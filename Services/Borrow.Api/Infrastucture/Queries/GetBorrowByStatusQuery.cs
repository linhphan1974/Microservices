
namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByStatusQuery : IRequest<List<BorrowDto>>
    {
        public int Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public GetBorrowByStatusQuery(int status, int pageIndex, int pageSize)
        {
            Status = status;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
