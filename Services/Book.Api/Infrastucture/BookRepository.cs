using BookOnline.Book.Api.Extension;
using BookOnline.Book.Api.Infrastucture.Data;
using BookOnline.Book.Api.Models;
using BookOnline.Book.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookOnline.Book.Api.Infrastucture
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDBContext _context;
        private readonly ILogger<IBookRepository> _logger;
        private readonly IOptions<BookSettings> _settings;

        public BookRepository(BookDBContext context, ILogger<BookRepository> logger, IOptions<BookSettings> settings)
        {
            _context = context;
            _logger = logger;
            _settings = settings;
        }

        #region Book type
        public async Task<BookType> GetBookTypeById(int id)
        {
            try
            {
                var bookType = await _context.BookTypes.FirstOrDefaultAsync(c => c.Id == id);
                _logger.LogInformation($"Get book type by id:{id} successfully!");
                return bookType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book type by id:{id} failure!");
            }

            return null;
        }

        public async Task<BookType> GetBookTypeByName(string name)
        {
            try
            {
                var bookType = await _context.BookTypes.FirstOrDefaultAsync(c => c.Name == name);
                _logger.LogInformation($"Get book type by name:{name} successfully!");
                return bookType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book type by name:{name} failure!");
            }

            return null;
        }

        public async Task<BookType> GetBookTypeByType(int type)
        {
            try
            {
                var bookType = await _context.BookTypes.FirstOrDefaultAsync(c => c.Type == type);
                _logger.LogInformation($"Get book type by type:{type} successfully!");
                return bookType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book type by type:{type} failure!");
            }

            return null;
        }

        public async Task<List<BookType>> GetBookTypes()
        {
            try
            {
                var bookTypes = await _context.BookTypes.ToListAsync();
                _logger.LogInformation($"Get book types successfully!");
                return bookTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book types failure!");
            }

            return null;
        }

        #endregion

        #region Book catalog
        public async Task<BookCatalog> GetBookCatalogById(int id)
        {
            try
            {
                var bookCatalog = await _context.BookCatalogs.FirstOrDefaultAsync(c => c.Id == id);
                _logger.LogInformation($"Get book catalog by id:{id} successfully!");
                return bookCatalog;
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Get book catalog by id:{id} failure!");
            }

            return null;
        }

        public async Task<BookCatalog> GetBookCatalogByName(string name)
        {
            try
            {
                var bookCatalog = await _context.BookCatalogs.FirstOrDefaultAsync(c => c.Name == name);
                _logger.LogInformation($"Get book catalog by name:{name} successfully!");
                return bookCatalog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book catalog by name:{name} failure!");
            }

            return null;
        }

        public async Task<List<BookCatalog>> GetBookCatalogs()
        {
            try
            {
                var bookCatalog = await _context.BookCatalogs.ToListAsync();
                _logger.LogInformation($"Get book catalog list successfully!");
                return bookCatalog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book catalog list failure!");
            }

            return null;
        }

        #endregion

        #region Book item
        public async Task<List<BookItem>> GetBookItemByAuthor(string authorName)
        {
            try
            {
                var bookItems = await _context.BookItems.Include(i => i.Catalog).Include(t => t.BookType).Where(c => c.Author == authorName).ToListAsync();
                _logger.LogInformation($"Get book item by author:{authorName} successfully!");
                
                bookItems = ChangeUriPlaceholder(bookItems);

                return bookItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book items by name:{authorName} failure!");
            }

            return null;
        }

        public async Task<BookItem> GetBookItemById(int id)
        {
            try
            {
                var book = await _context.BookItems.Include(i=>i.Catalog).Include(t=>t.BookType).FirstOrDefaultAsync(c => c.Id== id);
                book = ChangeUriItemPlaceholder(book); 
                _logger.LogInformation($"Get book item by id:{id} successfully!");
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book item by id:{id} failure!");
            }

            return null;
        }

        public async Task<List<BookItem>> GetBookItemByTitle(string title)
        {
            try
            {
                var bookItems = await _context.BookItems.Include(i => i.Catalog).Include(t => t.BookType).Where(c => c.Title.Contains(title)).ToListAsync();
                bookItems = ChangeUriPlaceholder(bookItems);
                _logger.LogInformation($"Get book items by title:{title} successfully!");
                return bookItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book items by title:{title} failure!");
            }

            return null;
        }

        public async Task<List<BookItem>> GetBookItems()
        {
            try
            {
                var bookItems = await _context.BookItems.Include(i => i.Catalog).Include(t => t.BookType).ToListAsync();
                bookItems = ChangeUriPlaceholder(bookItems);
                _logger.LogInformation($"Get book items successfully!");
                return bookItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book items failure!");
            }

            return null;
        }

        public async Task<PaginatedItemsViewModel<BookItem>> GetBookItems(int? type, int? catalog, bool? isAvailable, int pageSize, int pageIndex)
        {
            var root = (IQueryable<BookItem>)_context.BookItems;
            root = root.Include(i => i.Catalog).Include(e => e.BookType);

            if(type.HasValue)
            {
                root = root.Where(b => b.BookTypeId == type.Value);
            }

            if(catalog.HasValue)
            {
                root = root.Where(b => b.CatalogId == catalog.Value);
            }

            if (isAvailable.HasValue)
            {
                if (isAvailable.Value)
                {
                    root = root.Where(b => b.Quantity != 0);
                }
                else
                {
                    root = root.Where(b => b.Quantity == 0);
                }
            }

            var count = await root.LongCountAsync();
            var result = await root.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            
            result = ChangeUriPlaceholder(result);

            return new PaginatedItemsViewModel<BookItem>(pageIndex, pageSize, count, result);
        }

        public async Task<bool> UpdateBooksQuality(List<int> ids, BookStatus status)
        {
            try
            {
                var books = await _context.BookItems.Where(b => ids.Contains(b.Id)).ToListAsync();
                if(books.Count() > 0)
                {
                    foreach(var book in books)
                    {
                        if (status == BookStatus.Return || status == BookStatus.Cancelled)
                            book.Quantity++;
                        else
                            book.Quantity--;
                    }

                    await _context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update book item quality by ids fail!");
            }

            return false;

        }

        public async Task<List<BookItem>> GetBookItems(List<int> ids)
        {
            try
            {
                var bookItems = await _context.BookItems.Include(i => i.Catalog).Include(t => t.BookType).Where(b => ids.Contains(b.Id)).ToListAsync();
                bookItems = ChangeUriPlaceholder(bookItems);
                _logger.LogInformation($"Get book items successfully!");
                return bookItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book items failure!");
            }

            return null;
        }

        public async Task<long> GetLongCount()
        {
            return await _context.BookItems.LongCountAsync();
        }
        public async Task<bool> ConfirmedBookAvailable(int id)
        {
            try
            {
                var book = await _context.BookItems.FirstOrDefaultAsync(c => c.Id == id && c.Quantity > 0);
                _logger.LogInformation($"Get book item by id:{id} successfully!");
                return book is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get book item by id:{id} failure!");
            }

            return false;
        }
        #endregion

        private List<BookItem> ChangeUriPlaceholder(List<BookItem> items)
        {
            var baseUri = _settings.Value.PictureUrl;

            foreach (var item in items)
            {
                item.PictureUrl = baseUri.Replace("[0]", item.Id.ToString());
            }

            return items;
        }
        private BookItem ChangeUriItemPlaceholder(BookItem item)
        {
            var baseUri = _settings.Value.PictureUrl;

            item.PictureUrl = baseUri.Replace("[0]", item.Id.ToString());

            return item;
        }
    }
}
