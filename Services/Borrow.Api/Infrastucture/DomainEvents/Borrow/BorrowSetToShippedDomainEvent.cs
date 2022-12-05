namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowSetToShippedDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }
        public BorrowSetToShippedDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
