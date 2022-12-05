using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.Borrowing.Api.Service;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.EventHandlers
{
    public class BorrowProcessStartedEventHandler : IEventHandler<BorrowProcessStartedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BorrowProcessStartedEventHandler> _logger;
        private readonly IBorrowingApplicationEventService _eventLog;
        public BorrowProcessStartedEventHandler(IMediator mediator, ILogger<BorrowProcessStartedEventHandler> logger, IBorrowingApplicationEventService eventLog)
        {
            _mediator = mediator;
            _logger = logger;
            _eventLog = eventLog;
        }

        public async Task Handler(BorrowProcessStartedEvent @event)
        {
            BorrowChangeStatusToWaitForConfirmCommand command = new BorrowChangeStatusToWaitForConfirmCommand(@event.BorrowId);
            await _mediator.Send(command);
            _logger.LogTrace("Update borrow status to confirmed");
        }
    }
}
