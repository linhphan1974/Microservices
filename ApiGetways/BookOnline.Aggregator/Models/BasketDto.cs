namespace BookOnline.Aggregator.Models
{
    public class BasketDto
    {
        public string BasketId { get; set; }
        public List<BasketItemDto> Items { get; set; }

        public BasketDto()
        {
            Items = new List<BasketItemDto>();
        }

    }
}
