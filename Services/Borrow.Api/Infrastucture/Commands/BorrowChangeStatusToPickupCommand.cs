using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToPickupCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public BorrowChangeStatusToPickupCommand(int id)
        {
            Id = id;
        }
    }
}
