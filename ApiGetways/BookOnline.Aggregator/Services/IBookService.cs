using BookOnline.Aggregator.Models;

namespace BookOnline.Aggregator.Services
{
    public interface IBookService
    {
        Task<BookItemDto> GetBookByIdAsync(int id);
        Task<BookTypeDto> GetBookTypeByIdAsync(int id);
        Task<BookCatalogDto> GetBookCatalogByIdAsync(int id);
    }
}
