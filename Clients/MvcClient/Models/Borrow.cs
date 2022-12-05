using System.ComponentModel.DataAnnotations;

namespace BookOnline.MvcClient.Models
{
    public class Borrow
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        [Display(Name = "Borrow Date")]
        public DateTime BorrowDate { get; set; }
        [Display(Name ="Status")]
        public int BorrowStatus { get; set; }
        [Display(Name ="Date Pickup")]
        public DateTime PickupDate { get; set; }
        [Display(Name ="Date Returned")]
        public DateTime ReturnDate { get; set; }
        [Display(Name ="Date Cancelled")]
        public DateTime CancelDate { get; set; }
        public string Description { get; set; }
        public int ShipType { get; set; }
        public Address Address { get; set; }
        public List<BorrowItem> Items { get; set; }
        public bool IsShip { get; set; }
        public Borrow()
        {
            IsShip = false;
            Items = new List<BorrowItem>();
        }
    }

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
        WaitForShip = 8,
        Shipped = 9,
        Delivered = 10

    }
    public enum ShipType
    {
        Pickup = 0,
        Shipping = 1
    }

}
