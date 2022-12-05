using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Service
{
    public interface IBorrowingApplicationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(ApplicationEvent evt);
        Task DeleteCurrentPendingEvent();
    }
}
