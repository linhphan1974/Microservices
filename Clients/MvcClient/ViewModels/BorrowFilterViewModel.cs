using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.ViewModels
{
    public class BorrowFilterViewModel
    {
        public int? Status { get; set; } = 2;
        public DateTime? BorrowDate { get; set; }
        public PagingModel SearchModel { get; set; }

        public BorrowFilterViewModel()
        {
            SearchModel = new PagingModel();
        }
    }
}
