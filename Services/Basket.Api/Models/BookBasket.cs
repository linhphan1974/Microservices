namespace BookOnline.Basket.Api.Models
{
    public class BookBasket
    {
        public string MemberId { get; set; }

        public List<BasketItem> Items { get; set; } = new();

        public BookBasket(string memberId)
        {
            MemberId = memberId;
        }
    }
}
