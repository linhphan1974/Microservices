using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByMemberQuery : IRequest<List<BorrowDto>>
    {
        public int MemberId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public GetBorrowByMemberQuery(int memberId, int pageIndex, int pageSize)
        {
            MemberId = memberId;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
