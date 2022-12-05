using BookOnline.BorrowNotification.Api.Application.Events;
using Microsoft.AspNetCore.SignalR;
using RabbitMQEventBus;
using Serilog.Context;

namespace BookOnline.BorrowNotification.Api.Application.EventHandlers
{
    public class BorrowChangeStatusToConfirmedEventHandler : IEventHandler<BorrowChangeStatusToConfirmedEvent>
    {
        private readonly IHubContext<NotificationHub> _context;
        private readonly ILogger<BorrowChangeStatusToConfirmedEventHandler> _logger;
        public BorrowChangeStatusToConfirmedEventHandler(IHubContext<NotificationHub> context, ILogger<BorrowChangeStatusToConfirmedEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handler(BorrowChangeStatusToConfirmedEvent @event)
        {
            using (LogContext.PushProperty("EventContext", $"{@event.Id}-{typeof(Program).GetType().Name}"))
            {
                _logger.LogInformation("----- Handling integration event: {ApplicationEventId} at {AppName} - ({@ApplicationEvent})", @event.Id, typeof(Program).GetType().Name, @event);

                await _context.Clients
                    .Group(@event.MemberName)
                    .SendAsync("UpdatedBorrowState", new { BorrowId = @event.BorrowId, Status = @event.Status });
            }
        }
    }
}
