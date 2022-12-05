using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowReturnedDomainEvent : INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowReturnedDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
