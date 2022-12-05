namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowSetWaitForShipDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }
        public BorrowSetWaitForShipDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
