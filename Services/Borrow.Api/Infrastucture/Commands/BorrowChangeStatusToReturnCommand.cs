using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToReturnCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public BorrowChangeStatusToReturnCommand(int id)
        {
            Id = id;
        }
    }
}
