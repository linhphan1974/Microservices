using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Service;
using BookOnline.EventLogService.Services;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class BorrowCancelCommandHandler : IRequestHandler<BorrowCancelCommand, bool>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IBorrowRepository _repository;
        private readonly IBorrowingApplicationEventService _eventLog;
        public BorrowCancelCommandHandler(ILoggerFactory loggerFactory, IBorrowRepository repository, IBorrowingApplicationEventService eventLog)
        {
            _loggerFactory = loggerFactory;
            _repository = repository;
            _eventLog = eventLog;
        }

        public async Task<bool> Handle(BorrowCancelCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _repository.GetByIdAsync(request.Id);
            
            if(borrow != null)
            {
                borrow.SetBorrowCancel();
                //_repository.Update(borrow);
                await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return true;
            }

            return false;
        }
    }
}
