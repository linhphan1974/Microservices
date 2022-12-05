namespace BookOnline.Ordering.Api.Models
{
    public enum OrderStatus
    {
        WaitForStockConfirm = 1,
        WaitForPickup = 2,
        Pickedup = 3,
        Returned = 4
    }
}
