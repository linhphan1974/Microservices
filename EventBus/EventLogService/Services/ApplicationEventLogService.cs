using BookOnline.EventLogService;
using BookOnline.EventLogService.Entities;
using BookOnline.EventLogService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookOnline.EventServiceLog.Services
{
    public class ApplicationEventLogService : IApplicationEventLogService, IDisposable
    {
        private readonly EventLogContext _eventLogContext;
        private readonly DbConnection _dbConnection;
        private readonly List<Type> _eventTypes;
        private volatile bool _disposedValue;

        public ApplicationEventLogService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _eventLogContext = new EventLogContext(
                new DbContextOptionsBuilder<EventLogContext>()
                    .UseSqlServer(_dbConnection)
                    .Options);

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes().ToList();
                //.Where(t => t.Name.EndsWith(nameof(ApplicationEvent))).ToList();
                
        }

        public async Task<IEnumerable<ApplicationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            try
            {
                var tid = transactionId.ToString();

                var result = await _eventLogContext.EventLogs
                    .Where(e => e.TransactionId == tid && e.State == EventStatus.NotPublished).ToListAsync();

                if (result != null && result.Any())
                {
                    var data = result.OrderBy(o => o.CreationTime)
                        .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));

                    return data;
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return new List<ApplicationEventLog>();
        }

        public async Task SaveEventAsync(ApplicationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new ApplicationEventLog(@event, transaction.TransactionId);

            _eventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _eventLogContext.EventLogs.Add(eventLogEntry);

            await _eventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStatus.Published);
        }
        public Task DeleteEventAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntries = _eventLogContext.EventLogs.Where(e=>e.State == EventStatus.NotPublished && e.TransactionId == transaction.TransactionId.ToString());

            _eventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _eventLogContext.EventLogs.RemoveRange(eventLogEntries);

            return _eventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStatus.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStatus.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStatus status)
        {
            var eventLogEntry = _eventLogContext.EventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStatus.InProgress)
                eventLogEntry.TimesSent++;

            _eventLogContext.EventLogs.Update(eventLogEntry);

            return _eventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _eventLogContext?.Dispose();
                }


                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
