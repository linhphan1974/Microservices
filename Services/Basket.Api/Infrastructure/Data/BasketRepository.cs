using BookOnline.Basket.Api.Models;
using System.Text.Json;

namespace BookOnline.Basket.Api.Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public BasketRepository(ILogger<BasketRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string memberId)
        {
            return await _database.KeyDeleteAsync(memberId);
        }

        public async Task<BookBasket> GetBasketAsync(string memberId)
        {
            var data = await _database.StringGetAsync(memberId);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<BookBasket>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<BookBasket> UpdateBasketAsync(BookBasket basket)
        {
            var created = await _database.StringSetAsync(basket.MemberId, JsonSerializer.Serialize(basket));

            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetBasketAsync(basket.MemberId);
        }
    }
}
