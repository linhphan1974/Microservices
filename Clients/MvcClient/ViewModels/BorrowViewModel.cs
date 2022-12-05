using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.ViewModels
{
    public class BorrowViewModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public int TotalPage { get; set; }

        public List<Borrow> Data { get; set; }

        public BorrowViewModel()
        {

        }
    }
}
