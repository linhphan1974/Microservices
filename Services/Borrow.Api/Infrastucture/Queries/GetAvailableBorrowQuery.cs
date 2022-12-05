namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetAvailableBorrowQuery : IRequest<List<BorrowDto>>
    {
        public int MemberId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public GetAvailableBorrowQuery(int memberId, int pageIndex, int pageSize)
        {
            MemberId = memberId;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
