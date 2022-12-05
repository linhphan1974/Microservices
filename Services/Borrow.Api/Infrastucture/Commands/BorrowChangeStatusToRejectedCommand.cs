using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToRejectedCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }

        public BorrowChangeStatusToRejectedCommand(int borrowId)
        {
            this.BorrowId = borrowId;
        }
    }
}
