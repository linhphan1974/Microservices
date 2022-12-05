using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowSetWaitForConfirmDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowSetWaitForConfirmDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
