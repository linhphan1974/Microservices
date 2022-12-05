using BookOnline.EventLogService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQEventBus;
using System.Data.Common;

namespace BookOnline.Borrowing.Api.Service
{
    public class BorrowingApplicationEventService : IBorrowingApplicationEventService
    {
        private readonly Func<DbConnection, IApplicationEventLogService> _eventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly IApplicationEventLogService _eventLogService;
        private readonly ILogger<BorrowingApplicationEventService> _logger;
        private readonly BorrowingDBContext _context;
        public BorrowingApplicationEventService(Func<DbConnection, 
            IApplicationEventLogService> eventLogServiceFactory,
            IEventBus eventBus,
            ILogger<BorrowingApplicationEventService> logger, 
            BorrowingDBContext context)
        {
            _eventLogServiceFactory = eventLogServiceFactory;
            _eventBus = eventBus;
            _context = context;
            _eventLogService = _eventLogServiceFactory(_context.Database.GetDbConnection());
            _logger = logger;
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            try
            {
                var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

                foreach (var logEvt in pendingLogEvents)
                {
                    _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, typeof(Program).Name, logEvt.ApplicationEvent);

                    try
                    {
                        await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                        _eventBus.Publish(logEvt.ApplicationEvent);
                        await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, typeof(Program).Name);

                        await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public async Task AddAndSaveEventAsync(ApplicationEvent evt)
        {
            _logger.LogInformation("Save event to event log database", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _context.GetCurrentTransaction());
        }

        public async Task DeleteCurrentPendingEvent()
        {
            _logger.LogInformation("Delete pending event");

            await _eventLogService.DeleteEventAsync(_context.GetCurrentTransaction());

        }
    }
}
