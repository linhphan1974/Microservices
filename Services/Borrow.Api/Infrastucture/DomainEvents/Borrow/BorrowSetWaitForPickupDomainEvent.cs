using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowSetWaitForPickupDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowSetWaitForPickupDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
