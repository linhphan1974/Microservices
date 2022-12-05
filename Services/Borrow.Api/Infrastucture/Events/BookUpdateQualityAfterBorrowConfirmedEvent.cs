using RabbitMQEventBus;

namespace BookOnline.Book.Api.Infrastucture.Events
{
    public record BookUpdateQualityAfterBorrowConfirmedEvent : ApplicationEvent
    {
        public List<int> BookIds { get; set; }

        public BookUpdateQualityAfterBorrowConfirmedEvent(List<int> ids)
        {
            BookIds = ids;
        }
    }
}
