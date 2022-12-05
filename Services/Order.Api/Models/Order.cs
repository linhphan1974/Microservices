namespace BookOnline.Ordering.Api.Models
{
    public class Order : Entity
    {
        public string MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public int OrderStatus { get; set; }
        public List<OrderItem> Items { get; set; }

    }
}
