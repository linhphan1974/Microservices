using BookOnline.Book.Api.Infrastucture.Events;
using BookOnline.Book.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.EventHandlers
{
    public class BookUpdateQualityAfterBorrowCancelEventHandler : IEventHandler<BorrowCancelEvent>
    {
        private readonly ILogger<BookUpdateQualityAfterBorrowCancelEventHandler> _logger;
        private readonly IBookRepository _bookRepository;

        public BookUpdateQualityAfterBorrowCancelEventHandler(ILogger<BookUpdateQualityAfterBorrowCancelEventHandler> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task Handler(BorrowCancelEvent @event)
        {
            await _bookRepository.UpdateBooksQuality(@event.BookIds, BookStatus.Cancelled);
            _logger.LogTrace("Book updated quanlity after borrow cancelled");
        }
    }
}
