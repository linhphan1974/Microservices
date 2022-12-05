
using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToPickupCommandHandler : IRequestHandler<BorrowChangeStatusToPickupCommand, bool>
    {
        private readonly ILogger<BorrowChangeStatusToPickupCommandHandler> _logger;
        private readonly IBorrowRepository _borrowRepository;
        public BorrowChangeStatusToPickupCommandHandler(IBorrowRepository borrowRepository, ILogger<BorrowChangeStatusToPickupCommandHandler> logger)
        {
            _borrowRepository = borrowRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToPickupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var borrow = await _borrowRepository.GetByIdAsync(request.Id);

                if(borrow is not null)
                {
                    borrow.SetBorrowPickup();
                    await _borrowRepository.UnitOfWork.SaveEntitiesAsync();

                    return true;
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Change borrow status to pickup fail");
            }
            return false;
        }
    }
}
