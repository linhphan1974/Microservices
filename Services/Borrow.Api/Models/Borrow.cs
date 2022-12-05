using BookOnline.Borrowing.Api.Infrastucture.DomainEvents;

namespace BookOnline.Borrowing.Api.Models
{
    public class Borrow : Entity, IRoot
    {
        public int? MemberId { get; set; }
        private DateTime _borrowDate { get; set; }
        private int _borrowStatus { get; set; }
        private DateTime _pickupDate { get; set; }
        private DateTime _shipDate { get; set; }
        private DateTime _returnDate { get; set; }
        private DateTime _cancelDate { get; set; }
        private string _description { get; set; }
        private int _shipType { get; set; }
        public Address Address { get; private set; }

        private readonly List<BorrowItem> _items;
        public IReadOnlyCollection<BorrowItem> Items => _items;

        private bool _isDraft;
        public Borrow()
        {
            _items = new List<BorrowItem>();
            _isDraft = false;
        }


        public Borrow(string userId, string username,Address address, int shipType)
        {
            _borrowDate = DateTime.UtcNow;
            _borrowStatus = (int)BorrowStatus.Submited;
            _items = new List<BorrowItem>();
            Address = address;
            _shipType = shipType;
            AddBorrowStartDomainEvent(userId, username);
        }

        public static Borrow NewDraft()
        {
            Borrow borrow = new Borrow();
            borrow._isDraft = true;

            return borrow;
        }
        public void AddBorrowItem(int bookId, string title, string pictureUrl)
        {
            var item = new BorrowItem(bookId, title, pictureUrl);
            _items.Add(item);
        }

        public int GetShipType()
        {
            return _shipType;
        }
        public void SetMemberId(int? memberId)
        {
            if(memberId.HasValue && memberId.Value == 0)
            {
                MemberId = null;
            }
            else
            {
                MemberId = memberId;
            }
        }

        public void SetBorrowPickup()
        {
            _pickupDate = DateTime.Now;
            _borrowStatus = (int)BorrowStatus.PickedUp;
            _description = "Books were picked up!";
        }

        public void SetBorrowShip()
        {
            _shipDate = DateTime.Now;
            _borrowStatus = (int)BorrowStatus.Shipped;
            _description = "Books were shipped!";

            AddDomainEvent(new BorrowSetToShippedDomainEvent(this));
        }
        public void SetBorrowReturn()
        {
            _returnDate = DateTime.UtcNow;
            _borrowStatus = (int)BorrowStatus.Returned;
            _description = "Books were returned!";
            AddDomainEvent(new BorrowReturnedDomainEvent(this));
        }

        public void SetBorrowCancel()
        {
            _cancelDate = DateTime.UtcNow;
            _borrowStatus = (int)BorrowStatus.Cancel;
            _description = "Borrow was cancelled!";
            AddDomainEvent(new BorrowCancelDomainEvent(this));
        }

        public void SetBorrowWaitForConfirm()
        {
            _borrowStatus = (int)BorrowStatus.WaitForConfirm;
            _description = "Borrow is waiting for stock confirmation!";
            AddDomainEvent(new BorrowSetWaitForConfirmDomainEvent(this));
        }

        public void SetBorrowToConfirmed()
        {
            _borrowStatus = (int)BorrowStatus.Confirmed;
            _description = "Borrow was confirmed!";
            AddDomainEvent(new BorrowConfirmedDomainEvent(this));
        }

        public void SetToWaitForPickup()
        {
            _borrowStatus = (int)BorrowStatus.WaitForPickup;
            _description = "Borrow is waiting for pickup!";
            AddDomainEvent(new BorrowSetWaitForPickupDomainEvent(this));
        }
        public void SetToWaitForShip()
        {
            _borrowStatus = (int)BorrowStatus.WaitForShip;
            _description = "Borrow is waiting for ship!";
            AddDomainEvent(new BorrowSetWaitForShipDomainEvent(this));
        }
        public void SetBorrowToRejected()
        {
            _borrowStatus = (int)BorrowStatus.Rejected;
            _description = "Borrow was rejected!";
            AddDomainEvent(new BorrowRejectedDomainEvent(this));
        }
        public void AddBorrowStartDomainEvent(string userId, string username)
        {
            _description = "Borrow was submitted";
            var createBorrowEvent = new BorrowCreateDomainEvent(userId, username, this);

            AddDomainEvent(createBorrowEvent);
        }
    }
}
