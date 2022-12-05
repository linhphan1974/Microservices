namespace BookOnline.Borrowing.Api.Models
{
    public class BorrowItem : Entity
    {
        public int BookId { get; set; }
        public int BorrowId { get; set; }
        private string _title;
        private string _pictureUrl;

        public BorrowItem(int bookId, string title, string pictureUrl)
        {
            BookId = bookId;
            _title = title;
            _pictureUrl = pictureUrl;
        }

        public string GetTitle()
        {
            return _title;
        }

        public string GetPictureUrl()
        {
            return _pictureUrl;
        }
    }
}
