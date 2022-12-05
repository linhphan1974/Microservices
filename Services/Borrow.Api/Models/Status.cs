namespace BookOnline.Borrowing.Api.Models
{
    public enum BorrowStatus
    {
        Submited = 0,
        WaitForConfirm = 1,
        Confirmed = 2,
        Rejected = 3,
        WaitForPickup = 4,
        PickedUp = 5,
        Returned = 6,
        Cancel = 7,
        WaitForShip =8,
        Shipped = 9,
        Delivered = 10
    }

    public enum MemberStatus
    {
        Locked = 0,
        Unlocked = 1
    }

    public enum ShipType
    {
        Pickup = 0,
        Shipping = 1
    }
}
