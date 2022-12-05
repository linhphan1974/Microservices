using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowChangeStatusToRejectedCommandHandler : IRequestHandler<BorrowChangeStatusToRejectedCommand, bool>
    {
        private readonly IBorrowRepository _repository;
        private readonly ILogger<BorrowChangeStatusToRejectedCommandHandler> _logger;

        public BorrowChangeStatusToRejectedCommandHandler(IBorrowRepository repository, 
            ILogger<BorrowChangeStatusToRejectedCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(BorrowChangeStatusToRejectedCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(request.BorrowId);
            
            if(borrow is not null)
            {
                borrow.SetBorrowToRejected();
                await _repository.UnitOfWork.SaveEntitiesAsync();

                return true;
            }

            return false;
        }
    }
}
