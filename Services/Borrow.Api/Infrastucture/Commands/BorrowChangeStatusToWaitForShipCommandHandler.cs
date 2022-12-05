using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForShipCommandHandler : IRequestHandler<BorrowChangeStatusToWaitForShipCommand, bool>
    {
        private readonly IBorrowRepository _repository;
        private readonly ILogger<BorrowChangeStatusToWaitForShipCommandHandler> _logger;
        public BorrowChangeStatusToWaitForShipCommandHandler(IBorrowRepository repository, 
            ILogger<BorrowChangeStatusToWaitForShipCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToWaitForShipCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var borrow = await _repository.GetByIdAsync(request.BorrowId);
                if (borrow != null)
                {
                    borrow.SetToWaitForPickup();
                    await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                    return true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Update borrow status fail!");
            }
            return false;
        }
    }
}
