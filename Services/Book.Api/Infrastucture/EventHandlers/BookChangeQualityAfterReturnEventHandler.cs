using BookOnline.Book.Api.Infrastucture.Events;
using BookOnline.Book.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.EventHandlers
{
    public class BookChangeQualityAfterReturnEventHandler : IEventHandler<BookChangeQualityAfterReturnEvent>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<BookChangeQualityAfterReturnEventHandler> _logger;

        public BookChangeQualityAfterReturnEventHandler(IBookRepository repository, ILogger<BookChangeQualityAfterReturnEventHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handler(BookChangeQualityAfterReturnEvent @event)
        {
            var books = await _repository.UpdateBooksQuality(@event.BookIds, BookStatus.Return);
            _logger.LogTrace("Update book quality after borrower return");
            
        }
    }
}
