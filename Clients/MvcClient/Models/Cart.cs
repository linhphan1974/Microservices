namespace BookOnline.MvcClient.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }
    }
}
