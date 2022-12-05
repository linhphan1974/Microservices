using BookOnline.Borrowing.Api.Infrastucture.Repositories;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToShipCommandHandler : IRequestHandler<BorrowChangeStatusToShipCommand, bool>
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly ILogger<BorrowChangeStatusToShipCommandHandler> _logger;
        public BorrowChangeStatusToShipCommandHandler(IBorrowRepository borrowRepository,
            ILogger<BorrowChangeStatusToShipCommandHandler> logger)
        {
            _borrowRepository = borrowRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToShipCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var borrow = await _borrowRepository.GetByIdAsync(request.BorrowId);

                if (borrow != null)
                {
                    borrow.SetBorrowShip();
                    await _borrowRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                }

                return true;
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, ex.Message);
            }

            return false;
        }
    }
}
