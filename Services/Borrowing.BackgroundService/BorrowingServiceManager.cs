using BookOnline.Borrowing.BackgroundTask.Events;
using Dapper;
using Microsoft.Extensions.Options;
using RabbitMQEventBus;
using System.Data.SqlClient;

namespace BookOnline.Borrowing.BackgroundTask
{
    public class BorrowingServiceManager : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly BackgroundTaskSettings _settings;
        private readonly ILogger<BorrowingServiceManager> _logger;

        public BorrowingServiceManager(IOptions<BackgroundTaskSettings> settings, ILogger<BorrowingServiceManager> logger, IEventBus eventBus)
        {
            _settings = settings? .Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
            _eventBus = eventBus;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background task start");

            stoppingToken.Register(() => _logger.LogInformation("Background task stop!"));

            while(!stoppingToken.IsCancellationRequested)
            {
                var ids = CheckOrderToProcess();

                foreach(var id in ids)
                {
                    BorrowProcessStartedEvent borrowProcessStartedEvent = new BorrowProcessStartedEvent(id);
                    _logger.LogInformation("----- Publishing application event: {EventId} from {AppName} - ({@Event})", borrowProcessStartedEvent.Id, typeof(Program).GetType().Name, borrowProcessStartedEvent);

                    _eventBus.Publish(borrowProcessStartedEvent);
                }

                await Task.Delay(_settings.CheckPeriod);
            }
        }

        private List<int> CheckOrderToProcess()
        {
            using(var connection = new SqlConnection(_settings.ConnectionString))
            {
                connection.Open();
                var ids = connection.Query<int>(
                    @"Select Id FROM Borrow WHERE BorrowStatus = 0 AND DATEDIFF(minute, BorrowDate, GETDATE()) >= @ConfirmTime", 
                    new { _settings.ConfirmTime });

                return ids.ToList();
            }
        }
    }
}
