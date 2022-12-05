using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.ViewModels
{
    public class BookResponseViewModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public IEnumerable<BookItemDto> Data { get; set; }
    }
}
