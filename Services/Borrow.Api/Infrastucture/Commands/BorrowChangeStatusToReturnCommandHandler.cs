using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToReturnCommandHandler : IRequestHandler<BorrowChangeStatusToReturnCommand, bool>
    {
        private readonly ILogger<BorrowChangeStatusToReturnCommandHandler> _logger;
        private readonly IBorrowRepository _borrowRepository;

        public BorrowChangeStatusToReturnCommandHandler(ILogger<BorrowChangeStatusToReturnCommandHandler> logger, IBorrowRepository borrowRepository)
        {
            _logger = logger;
            _borrowRepository = borrowRepository;
        }

        public async Task<bool> Handle(BorrowChangeStatusToReturnCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var borrow = await _borrowRepository.GetByIdAsync(request.Id);

                if (borrow is not null)
                {
                    borrow.SetBorrowReturn();
                    await _borrowRepository.UnitOfWork.SaveEntitiesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change borrow status to return fail");
            }
            return false;
        }
    }
}
