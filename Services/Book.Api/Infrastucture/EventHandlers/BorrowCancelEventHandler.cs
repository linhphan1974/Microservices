using BookOnline.Book.Api.Infrastucture.Events;
using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.EventHandlers
{
    public class BorrowCancelEventHandler : IEventHandler<BorrowCancelEvent>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BorrowCancelEventHandler> _logger;
        public async Task Handler(BorrowCancelEvent @event)
        {
            await _bookRepository.UpdateBooksQuality(@event.BookIds, Models.BookStatus.Cancelled);

            _logger.LogTrace("Update book quantity after cancel borrow");
        }
    }
}
