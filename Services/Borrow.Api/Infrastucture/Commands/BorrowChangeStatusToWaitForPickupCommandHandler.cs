using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForPickupCommandHandler : IRequestHandler<BorrowChangeStatusToWaitForPickupCommand, bool>
    {
        private readonly IBorrowRepository _repository;
        private readonly ILogger<BorrowChangeStatusToWaitForPickupCommandHandler> _logger;
        public BorrowChangeStatusToWaitForPickupCommandHandler(IBorrowRepository repository, ILogger<BorrowChangeStatusToWaitForPickupCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToWaitForPickupCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(request.BorrowId);

            if(borrow != null)
            {
                borrow.SetToWaitForPickup();
                await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return true;
            }

            return false;
        }
    }
}
