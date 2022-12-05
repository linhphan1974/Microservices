namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForPickupCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }

        public BorrowChangeStatusToWaitForPickupCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
