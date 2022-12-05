namespace BookOnline.Ordering.Api.Infrastructure.Events
{
    public class OrderStockRejectEvent
    {
        public int OrderId { get; set; }
        public List<ConfirmedBookItem> ConfirmedBookItems { get; set; }

        public OrderStockRejectEvent(int id, List<ConfirmedBookItem> items)
        {
            OrderId = id;
            ConfirmedBookItems = items;
        }
    }

    public class ConfirmedBookItem
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }

        public ConfirmedBookItem(int id, bool isAvailable)
        {
            id = Id;
            IsAvailable = isAvailable;
        }
    }
}
