
namespace BookOnline.Ordering.Api.Models
{
    public class OrderItem : Entity
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public Order Order { get; set; }

    }
}
