namespace BookOnline.Ordering.Api.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookCode { get; set; }
        public string PictureUrl { get; set; }

    }
}
