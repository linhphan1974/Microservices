using BookOnline.Borrowing.Api.Infrastucture.Data;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.Commands
{
    public class CreateBorrowCommandHandler : IRequestHandler<CreateBorrowCommand, bool>
    {
        private readonly ILogger<CreateBorrowCommandHandler> _logger;
        private readonly IBorrowingApplicationEventService _eventLogService;
        private readonly IBorrowRepository _borrowRepository;
        private readonly IMediator _mediator;

        public CreateBorrowCommandHandler(ILogger<CreateBorrowCommandHandler> logger,
            IBorrowingApplicationEventService eventLogService,
            IBorrowRepository borrowRepository, IMediator mediator)
        {
            _logger = logger;
            _eventLogService = eventLogService;
            _borrowRepository = borrowRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(CreateBorrowCommand request, CancellationToken cancellationToken)
        {
            //Call borrow created event to clear basket
            BorrowCreatedEvent borrowCreatedEvent = new BorrowCreatedEvent(request.UserId);
            await _eventLogService.AddAndSaveEventAsync(borrowCreatedEvent);
            
            try
            {
                var borrow = new Borrow(request.UserId, request.Username,request.Address, request.ShipType);
                foreach(var item in request.Basket.Items)
                {
                    borrow.AddBorrowItem(item.BookId, item.BookName, item.PictureUrl);
                }

                borrow = _borrowRepository.Create(borrow);

                //Send notification to signalR service and user
                BorrowChangestatusToSubmittedEvent borrowSubmittedEvent = new BorrowChangestatusToSubmittedEvent(borrow.Id, request.Username, "Your borrow was submitted!");
                await _eventLogService.AddAndSaveEventAsync(borrowSubmittedEvent);

                await _borrowRepository.UnitOfWork.SaveEntitiesAsync();

                return true;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Create borrow data fail");
            }

            return false;
        }
    }
}
