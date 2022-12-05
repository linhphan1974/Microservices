namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public class OrderStockConfirmEvent
    {
        public int OrderId { get; set; }

        public OrderStockConfirmEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
