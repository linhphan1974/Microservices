using BookOnline.Book.Api.Infrastucture.Events;
using BookOnline.Book.Api.Models;
using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.EventHandlers
{
    public class BookUpdateQualityAfterBorrowConfirmedEventHandler : IEventHandler<BookUpdateQualityAfterBorrowConfirmedEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBookRepository _bookRepository;

        public BookUpdateQualityAfterBorrowConfirmedEventHandler(ILoggerFactory logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task Handler(BookUpdateQualityAfterBorrowConfirmedEvent @event)
        {
            await _bookRepository.UpdateBooksQuality(@event.BookIds, BookStatus.BorrowCreated);
            _logger.CreateLogger<BookUpdateQualityAfterBorrowConfirmedEventHandler>().LogTrace("Update book quality after borrow data created!");
        }
    }
}
