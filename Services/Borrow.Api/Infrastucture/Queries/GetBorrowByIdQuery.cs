using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByIdQuery : IRequest<BorrowDto>
    {
        public int Id { get; set; }

        public GetBorrowByIdQuery(int id)
        {
            Id = id;
        }
    }


}
