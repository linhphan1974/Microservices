using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowCancelDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowCancelDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
