using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowCancelCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public BorrowCancelCommand(int id)
        {
            Id = id;
        }
    }
}
