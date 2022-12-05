using BookOnline.Book.Api.Infrastucture;
using BookOnline.Book.Api.Models;
using BookOnline.Book.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.Book.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBookRepository _repository;
        public BookController(ILogger<BookController> logger, IBookRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("items")]
        public async Task<ActionResult<List<BookItem>>> GetBookItems([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
        {
            if (ids is not null)
            {
                var bookItems = await _repository.GetBookItems();
                return Ok(bookItems);
            }

            var items = await _repository.GetBookItems(null, null, null, pageSize, pageIndex);

            return Ok(items);
        }

        [HttpGet]
        [Route("items/type/{typeId}/catalog/{catalogId}/available/{isAvailable:bool?}")]
        public async Task<ActionResult<PaginatedItemsViewModel<BookItem>>> GetBookItemsByTypeAndCatalog(int typeId, int catalogId, bool? isAvailable = null, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _repository.GetBookItems(typeId, catalogId, isAvailable, pageSize, pageIndex);
            return Ok(items);
        }

        [HttpGet]
        [Route("items/type/{typeId}/catalog/allcatalog/available/{isAvailable:bool?}")]
        public async Task<ActionResult<PaginatedItemsViewModel<BookItem>>> GetBookItemsByType(int typeId, bool? isAvailable = null, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _repository.GetBookItems(typeId, null, isAvailable, pageSize, pageIndex);
            return Ok(items);
        }

        [HttpGet]
        [Route("items/type/alltype/catalog/{catalogId}/available/{isAvailable:bool?}")]
        public async Task<ActionResult<PaginatedItemsViewModel<BookItem>>> GetBookItemsByCatalog(int catalogId, bool? isAvailable = null, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _repository.GetBookItems(null, catalogId, isAvailable, pageSize, pageIndex);
            return Ok(items);
        }

        [HttpGet]
        [Route("items/type/alltype/catalog/allcatalog/available/{isAvailable:bool?}")]
        public async Task<ActionResult<PaginatedItemsViewModel<BookItem>>> GetBookItemsByAvailable(bool? isAvailable = null, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _repository.GetBookItems(null, null, isAvailable, pageSize, pageIndex);
            return Ok(items);
        }

        [HttpGet]
        [Route("item/id/{id:int}")]
        public async Task<BookItem> GetBookItemById(int id)
        {
            return await _repository.GetBookItemById(id);
        }

        [HttpGet]
        [Route("items/title/{title}")]
        public async Task<List<BookItem>> GetBookItemsByTitle(string title)
        {
            return await _repository.GetBookItemByTitle(title);
        }

        [HttpGet]
        [Route("items/author/{author}")]
        public async Task<List<BookItem>> GetBookItemsByAuthor(string author)
        {
            return await _repository.GetBookItemByAuthor(author);
        }

        [HttpGet]
        [Route("catalog/id/{id:int}")]
        public async Task<BookCatalog> GetCatalogByIdAsync(int id)
        {
            var catalog = await _repository.GetBookCatalogById(id);
            return catalog;
        }

        [HttpGet]
        [Route("catalogs")]
        public async Task<List<BookCatalog>> GetBookCatalogsAsync()
        {
            return await _repository.GetBookCatalogs();
        }

        [HttpGet]
        [Route("type/id/{id:int}")]
        public async Task<BookType> GetBookTypeByIdAsync(int id)
        {
            return await _repository.GetBookTypeById(id);
        }

        [HttpGet]
        [Route("types")]
        public async Task<List<BookType>> GetBookTypes()
        {
            return await _repository.GetBookTypes();
        }


    }
}
