
using BookOnline.Aggregator.Models;
using GrpcBasket;

namespace BookOnline.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly Basket.BasketClient _client;
        private readonly ILogger<BasketService> _logger;
        public BasketService(Basket.BasketClient client, ILogger<BasketService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<BasketDto> GetBasketByIdAsync(string basketId)
        {
            var response = await _client.GetBasketByIdAsync(new BasketRequest { BasketId = basketId });

            var basket = new BasketDto { BasketId = response.BasketId };
            basket.Items.AddRange(response.Items.Select(i =>
                new BasketItemDto
                {
                    BookCode = i.BookCode,
                    BookId = i.BookId,
                    BookName = i.BookName,
                    Id = i.Id,
                    PictureUrl = i.PictureUrl
                }));

            _logger.LogTrace("Get basket data");
            return basket;
        }

        public async Task<BasketDto> UpdateBasketAsync(BasketDto basket)
        {
            var request = new UpdateBasketRequest { BasketId = basket.BasketId };
            request.Items.AddRange(basket.Items.Select(i =>
                new BasketItemResponse
                {
                    BookCode = i.BookCode,
                    BookId = i.BookId,
                    BookName = i.BookName,
                    Id = i.Id,
                    PictureUrl = i.PictureUrl
                }));

            _logger.LogTrace("Update basket data");
            var response = await _client.UpdateBasketAsync(request);

            var dto = new BasketDto
            {
                BasketId = response.BasketId
            };

            dto.Items.AddRange(response.Items.Select(i =>
                new BasketItemDto
                {
                    BookCode = i.BookCode,
                    BookId = i.BookId,
                    BookName = i.BookName,
                    Id = i.Id,
                    PictureUrl = i.PictureUrl
                }));

            return dto;
        }
    }
}
