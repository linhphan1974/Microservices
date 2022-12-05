using BookOnline.Book.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToConfirmedCommandHandler : IRequestHandler<BorrowChangeStatusToConfirmedCommand, bool>
    {
        private readonly IBorrowRepository _repository;
        private readonly ILogger<BorrowChangeStatusToConfirmedCommandHandler> _logger;

        public BorrowChangeStatusToConfirmedCommandHandler(IBorrowRepository repository,
            ILogger<BorrowChangeStatusToConfirmedCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToConfirmedCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(request.BorrowId);

            if(borrow is not null)
            {
                borrow.SetBorrowToConfirmed();
                await _repository.UnitOfWork.SaveEntitiesAsync();

                return true;
            }

            return false;
        }
    }
}
