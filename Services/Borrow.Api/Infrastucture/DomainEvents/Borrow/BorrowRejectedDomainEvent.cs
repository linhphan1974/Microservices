using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowRejectedDomainEvent: INotification
    {
        public Borrow Borrow { get; set; }

        public BorrowRejectedDomainEvent(Borrow borrow)
        {
            Borrow = borrow;
        }
    }
}
