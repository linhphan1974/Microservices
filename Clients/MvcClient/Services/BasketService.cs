using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Models.Dto;
using Microsoft.Extensions.Options;
using System.Net.Http.Formatting;
using System.Text.Json;

namespace BookOnline.MvcClient.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IOptions<AppSettings> _settings;
        private string _basketUrl;
        private string _aggregatorUrl;
        public BasketService(IHttpClientFactory httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _basketUrl = $"{settings.Value.BasketUrl}/api/basket";
            _aggregatorUrl = $"{settings.Value.AggregatorUrl}";
        }

        public async Task AddToCartAsync(string id, int bookId)
        {
            var url = UrlConfigs.Aggregator.AddToCart(_aggregatorUrl);
            AddToBasketRequestClient cart = new AddToBasketRequestClient { BookId = bookId, BasketId = id };

            var content = new StringContent(JsonSerializer.Serialize(cart), System.Text.Encoding.UTF8, "application/json");
            var client = _httpClient.CreateClient("MVCClient");
            await client.PostAsync<AddToBasketRequestClient>(url, cart, new JsonMediaTypeFormatter());
        }

        public async Task<bool> CartExist(string id)
        {
            var url = UrlConfigs.Aggregator.GetBasketByIdUrl(_aggregatorUrl, id);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<Cart>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result is not null;

        }

        public async Task<Cart> GetCurrentCartAsync(string id)
        {
            var url = UrlConfigs.Aggregator.GetBasketByIdUrl(_aggregatorUrl, id);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<Cart>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }
    }
}
