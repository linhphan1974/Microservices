using BookOnline.Aggregator.Models;
using Google.Protobuf.WellKnownTypes;
using GrpcBook;

namespace BookOnline.Aggregator.Services
{
    public class BookService : IBookService
    {
        private readonly Book.BookClient _client;
        private readonly ILogger<BookService> _logger;
        public BookService(Book.BookClient client, ILogger<BookService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<BookItemDto> GetBookByIdAsync(int id)
        {
            var response = await _client.GetBookByIdAsync(new BookRequest { BookId = id });

            _logger.LogTrace("Get book by id via grpc service");
            return new BookItemDto
            {
                Author = response.Author,
                BookType = new BookTypeDto { Id = response.BookType.Id, Name = response.BookType.Name, Type = response.BookType.Type },
                ISBN = response.ISBN,
                Id = response.Id,
                BookTypeId = response.BookTypeId,
                Catalog = new BookCatalogDto { Id = response.Catalog.Id, Name = response.Catalog.Name, Description = response.Catalog.Description },
                CatalogId = response.CatalogId,
                Description = response.Description,
                FirstPublish = response.FirstPublish.ToDateTime(),
                PictureUrl = response.PictureUrl,
                Publisher = response.Publisher,
                Quantity = response.Quantity,
                Status = response.Status,
                Title = response.Title,
                Version = response.Version
            };
        }

        public async Task<BookCatalogDto> GetBookCatalogByIdAsync(int id)
        {
            var response = await _client.GetBookCatalogByIdAsync(new BookCatalogRequest { BookCatalogId = id });

            _logger.LogTrace("Get book cayalog by id vi grpc service");
            return new BookCatalogDto
            {
                Description = response.Description,
                Id = response.Id,
                Name = response.Name
            };
        }

        public async Task<BookTypeDto> GetBookTypeByIdAsync(int id)
        {
            var response = await _client.GetBookTypeByIdAsync(new BookTypeRequest { BookTypeId = id });

            _logger.LogTrace("Get book type by id via grpc service");
            return new BookTypeDto
            {
                Id = response.Id,
                Name = response.Name,
                Type = response.Type
            };
        }
    }
}
