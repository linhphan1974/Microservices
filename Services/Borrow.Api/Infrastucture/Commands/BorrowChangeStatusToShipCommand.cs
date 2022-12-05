namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToShipCommand : IRequest<bool>
    {
        public int BorrowId { get; set; }
        public BorrowChangeStatusToShipCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
