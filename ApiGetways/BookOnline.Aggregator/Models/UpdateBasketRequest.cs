namespace BookOnline.Aggregator.Models
{
    public class UpdateBasketClientRequest
    {
        public string MemberId { get; set; }
        public string BasketId { get; set; }
        public List<BasketItemClientRequest> Items { get; set; }
    }

    public class BasketItemClientRequest
    {
        public string Id { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookCode { get; set; }
        public string PictureUrl { get; set; }

    }
}
