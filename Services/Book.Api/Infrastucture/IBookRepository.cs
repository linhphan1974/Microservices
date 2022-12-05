using BookOnline.Book.Api.Models;
using BookOnline.Book.Api.ViewModels;

namespace BookOnline.Book.Api.Infrastucture
{
    public interface IBookRepository
    {
        #region Book type
        Task<List<BookType>> GetBookTypes();

        Task<BookType> GetBookTypeById(int id);

        Task<BookType> GetBookTypeByName(string name);

        Task<BookType> GetBookTypeByType(int type);
        #endregion

        #region Book catalog
        Task<List<BookCatalog>> GetBookCatalogs();

        Task<BookCatalog> GetBookCatalogById(int id);

        Task<BookCatalog> GetBookCatalogByName(string name);
        #endregion

        #region Book Item
        Task<BookItem> GetBookItemById(int id);

        Task<List<BookItem>> GetBookItems();
        Task<List<BookItem>> GetBookItems(List<int> ids);

        Task<List<BookItem>> GetBookItemByTitle(string title);

        Task<List<BookItem>> GetBookItemByAuthor(string authorName);

        Task<PaginatedItemsViewModel<BookItem>> GetBookItems(int? type, int? catalog, bool? isAvailable,int pageSize,int pageIndex);

        Task<bool> UpdateBooksQuality(List<int> ids, BookStatus status);

        Task<long> GetLongCount();

        Task<bool> ConfirmedBookAvailable(int id);

        #endregion
    }
}
