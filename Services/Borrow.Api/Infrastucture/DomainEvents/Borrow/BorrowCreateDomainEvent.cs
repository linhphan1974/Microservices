using BookOnline.Borrowing.Api.Models;
using MediatR;

namespace BookOnline.Borrowing.Api.Infrastucture.DomainEvents
{
    public class BorrowCreateDomainEvent : INotification
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Borrow Borrow { get; set; }

        public BorrowCreateDomainEvent(string userId, string userName, Borrow borrow)
        {
            UserId = userId;
            UserName = userName;
            Borrow = borrow;
        }
    }
}
