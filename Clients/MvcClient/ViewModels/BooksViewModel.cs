using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.ViewModels
{
    public class BooksViewModel
    {
        public List<BookItemDto> BookItems { get; set; }
        public List<BookType> BookTypes { get; set; }
        public List<BookCatalog> BookCatalogs { get; set; }
        public BookCatalog SelectedCatalog { get; set; }
        public BookType SelectedType { get; set; }
        public PaginationViewModel PaginationInfo { get; set; }

        public BooksViewModel()
        {
            BookTypes=new List<BookType>();
            BookItems = new List<BookItemDto>();
            BookCatalogs = new List<BookCatalog>();
        }
    }
}
