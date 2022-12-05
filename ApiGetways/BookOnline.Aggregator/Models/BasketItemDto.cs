namespace BookOnline.Aggregator.Models
{
    public class BasketItemDto
    {
        public string Id { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookCode { get; set; }
        public string PictureUrl { get; set; }
    }
}
