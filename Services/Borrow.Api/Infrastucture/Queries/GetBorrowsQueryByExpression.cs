
namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowsQueryByExpression : IRequest<List<BorrowDto>>
    {
        public int? MemberId { get; set; }
        public int? Status { get; set; }
        public DateTime? BorrowDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public GetBorrowsQueryByExpression(int? memberId, int? status, DateTime? borrowDate, int pageIndex, int pageSize)
        {
            MemberId = memberId;
            Status = status;
            BorrowDate = borrowDate;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
