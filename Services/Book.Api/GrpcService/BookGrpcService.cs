using GrpcBook;
using BookOnline.Book.Api.Infrastucture;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static GrpcBook.Book;
using BookOnline.Book.Api.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;

namespace BookOnline.Book.Api.GrpcService
{
    public class BookGrpcService: BookBase
    {
        private readonly ILogger<BookGrpcService> _logger;
        private readonly IOptions<BookSettings> _settings;
        private readonly IBookRepository _bookRepository;
        public BookGrpcService(IBookRepository bookRepository, 
            ILogger<BookGrpcService> logger, 
            IOptions<BookSettings> settings)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _settings = settings;
        }

        public override async Task<BookResponse> GetBookById(BookRequest request, ServerCallContext context)
        {
            var book = await _bookRepository.GetBookItemById(request.BookId);

            _logger.LogTrace("Get book by id via grpc service");

            var response = new BookResponse
            {
                Author = book.Author,
                BookTypeId = book.BookTypeId,
                CatalogId = book.CatalogId,
                Description = book.Description,
                FirstPublish = Timestamp.FromDateTimeOffset(book.FirstPublish),
                ISBN = book.ISBN,
                PictureUrl = book.PictureUrl,
                Publisher = book.Publisher,
                Quantity = book.Quantity,
                Status = book.Status,
                Title = book.Title,
                Version = book.Version,
                Id = book.Id,
                BookType = new BookTypeResponse { Id = book.BookTypeId, Name = book.BookType.Name, Type = book.BookType.Type },
                Catalog = new BookCatalogResponse { Name = book.Catalog.Name, Id = book.Catalog.Id, Description = book.Catalog.Description }
            };

            return response;
        }

        //public override async Task<BookCatalogResponse> GetBookCatalogById(BookCatalogRequest request, ServerCallContext context)
        //{
        //    var catalog = await _repository.GetBookCatalogById(request.BookCatalogId);

        //    _logger.LogTrace("Get book catalog by id via grpc service");
        //    return new BookCatalogResponse
        //    {
        //        Description = catalog.Description,
        //        Name = catalog.Name
        //    };
        //}

        //public override async Task<BookTypeResponse> GetBookTypeById(BookTypeRequest request, ServerCallContext context)
        //{
        //    var type = await _repository.GetBookTypeById(request.BookTypeId);

        //    _logger.LogTrace("Get book type by id via grpc service");

        //    return new BookTypeResponse
        //    {
        //        Name = type.Name,
        //        Type = type.Type
        //    };
        //}
    }
}
