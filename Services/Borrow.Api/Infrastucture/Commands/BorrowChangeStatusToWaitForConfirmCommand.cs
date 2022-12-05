using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForConfirmCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }

        public BorrowChangeStatusToWaitForConfirmCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
