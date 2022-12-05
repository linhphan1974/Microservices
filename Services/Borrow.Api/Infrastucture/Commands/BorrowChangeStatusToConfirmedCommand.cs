using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToConfirmedCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }

        public BorrowChangeStatusToConfirmedCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
