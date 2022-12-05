using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowConfirmedDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowConfirmedDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
