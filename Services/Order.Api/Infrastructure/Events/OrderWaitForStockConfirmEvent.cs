using RabbitMQEventBus;

namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public record OrderWaitForStockConfirmEvent : ApplicationEvent
    {
        public int OrderId { get; set; }
        public List<OrderBookItem> Items { get; set; }

        public OrderWaitForStockConfirmEvent(int orderId, List<OrderBookItem> items)
        {
            Items = Items;
        }
    }

    public class OrderBookItem
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }

        public OrderBookItem(int id, bool isAvailable)
        {
            id = Id;
            IsAvailable = isAvailable;
        }
    }
}
