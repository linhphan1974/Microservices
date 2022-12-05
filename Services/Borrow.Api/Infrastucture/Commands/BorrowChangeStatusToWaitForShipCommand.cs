namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForShipCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }
        public BorrowChangeStatusToWaitForShipCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
