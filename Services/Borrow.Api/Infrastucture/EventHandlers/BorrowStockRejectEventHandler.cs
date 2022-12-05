using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using MediatR;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.EventHandlers
{
    public class BorrowStockRejectEventHandler : IEventHandler<BorrowStockRejectEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BorrowStockRejectEventHandler> _logger;

        public BorrowStockRejectEventHandler(IMediator mediator, ILogger<BorrowStockRejectEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handler(BorrowStockRejectEvent @event)
        {
            BorrowChangeStatusToRejectedCommand command = new BorrowChangeStatusToRejectedCommand(@event.BorrowId);
            await _mediator.Send(command);
        }
    }
}
