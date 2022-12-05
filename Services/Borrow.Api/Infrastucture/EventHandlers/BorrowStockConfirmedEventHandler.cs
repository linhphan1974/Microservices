using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using MediatR;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.EventHandlers
{
    public class BorrowStockConfirmedEventHandler : IEventHandler<BorrowStockConfirmedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BorrowStockConfirmedEventHandler> _logger;

        public BorrowStockConfirmedEventHandler(IMediator mediator, ILogger<BorrowStockConfirmedEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handler(BorrowStockConfirmedEvent @event)
        {
            //Call confirmed command
            BorrowChangeStatusToConfirmedCommand command = new BorrowChangeStatusToConfirmedCommand(@event.BorrowId);
            await _mediator.Send(command);
        }
    }
}
