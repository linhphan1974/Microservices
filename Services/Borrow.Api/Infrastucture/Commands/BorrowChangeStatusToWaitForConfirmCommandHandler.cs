using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToWaitForConfirmCommandHandler : IRequestHandler<BorrowChangeStatusToWaitForConfirmCommand, bool>
    {
        private readonly IBorrowRepository _repository;
        private readonly ILogger<BorrowChangeStatusToWaitForConfirmCommandHandler> _logger;

        public BorrowChangeStatusToWaitForConfirmCommandHandler(IBorrowRepository repository, ILogger<BorrowChangeStatusToWaitForConfirmCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToWaitForConfirmCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(request.BorrowId);

            if(borrow is not null)
            {
                borrow.SetBorrowWaitForConfirm();
                await _repository.UnitOfWork.SaveEntitiesAsync();

                return true;
            }

            return false;
        }
    }
}
