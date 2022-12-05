using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.Events
{
    public record BookChangeQualityAfterReturnEvent : ApplicationEvent
    {
        public List<int> BookIds { get; set; }

        public BookChangeQualityAfterReturnEvent(List<int> bookIds)
        {
            BookIds = bookIds;
        }
    }
}
