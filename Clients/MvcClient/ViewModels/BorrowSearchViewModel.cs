using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.ViewModels
{
    public class BorrowSearchViewModel
    {
        public DateTime? BorrowDate { get; set; }
        public int? Status { get; set; }

        public Dictionary<int,string> StatusList { get; set; }

        public List<Borrow> Borrows { get; set; }

        public PagingModel Pagination { get; set; }

        public BorrowSearchViewModel()
        {
            StatusList = new Dictionary<int, string>();

            StatusList.Add((int)BorrowStatus.Submited, "Submitted");
            StatusList.Add((int)BorrowStatus.WaitForConfirm, "Wait for confirm");
            StatusList.Add((int)BorrowStatus.Confirmed, "Confirmed");
            StatusList.Add((int)BorrowStatus.Rejected, "Rejected");
            StatusList.Add((int)BorrowStatus.WaitForPickup, "Wait for pickup");
            StatusList.Add((int)BorrowStatus.PickedUp, "Picked up");
            StatusList.Add((int)BorrowStatus.Returned, "Returned");
            StatusList.Add((int)BorrowStatus.Cancel, "Cancelled");
            StatusList.Add((int)BorrowStatus.WaitForShip, "Wait for shipping");
            StatusList.Add((int)BorrowStatus.Shipped, "Shipped");
            StatusList.Add((int)BorrowStatus.Delivered, "Delivered");

            Borrows = new List<Borrow>();
        }
    }
}
