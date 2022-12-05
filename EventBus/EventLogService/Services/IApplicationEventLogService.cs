using BookOnline.EventLogService.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using RabbitMQEventBus;

namespace BookOnline.EventLogService.Services
{
    public interface IApplicationEventLogService
    {
        Task<IEnumerable<ApplicationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        Task SaveEventAsync(ApplicationEvent @event, IDbContextTransaction transaction);
        Task DeleteEventAsync(IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
