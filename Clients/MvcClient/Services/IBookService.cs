using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.ViewModels;

namespace BookOnline.MvcClient.Services
{
    public interface IBookService
    {
        Task<List<BookType>> GetBookTypesAsync();
        Task<BookType> GetBookTypeByIdAsync(int id);
        Task<BookCatalog> GetBookCatalogByIdAsync(int id);
        Task<List<BookCatalog>> GetBookCatalogsAsync(); 
        Task<BookItemDto> GetBookByIdAsync(int id);
        Task<BookResponseViewModel> GetBooksAsync(int pageIndex, int pageSize, int? bookType, int? catalogId, bool? byAvailable); 
    }
}
