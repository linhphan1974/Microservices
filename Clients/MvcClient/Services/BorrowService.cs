using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Models.Dto;
using Microsoft.Extensions.Options;
using System.Net.Http.Formatting;
using System.Text.Json;
using BookOnline.MvcClient.ViewModels;
using X.PagedList;

namespace BookOnline.MvcClient.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IOptions<AppSettings> _settings;
        private string _aggregatorUrl;
        private string _borrowUrl;
        private string _basketUrl;
        public BorrowService(IHttpClientFactory httpClient, 
            IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _borrowUrl = $"{_settings.Value.BorrowUrl}/api/borrow";
            _aggregatorUrl = $"{settings.Value.AggregatorUrl}";
            _basketUrl = $"{settings.Value.BasketUrl}/api/basket";
        }

        public async Task<bool> Checkout(Borrow borrow)
        {
            var url = UrlConfigs.Basket.Checkout(_basketUrl);
            CheckoutRequestClient cart = new CheckoutRequestClient { ShipType = borrow.ShipType};

            var content = new StringContent(JsonSerializer.Serialize(cart), System.Text.Encoding.UTF8, "application/json");
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PostAsync<CheckoutRequestClient>(url, cart, new JsonMediaTypeFormatter());

            return true;
        }

        public async Task<IPagedList<Borrow>> GetActiveBorrowsAsync(string memberId)
        {
            var url = UrlConfigs.Borrow.GetAvailableBorrowsUrl(_borrowUrl, memberId, 1, 0);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<Borrow>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return await result.ToPagedListAsync(1,1);
        }

        public async Task<IPagedList<Borrow>> GetBorrowsAsync(string memberId, int? status, DateTime? borrowDate, int pageIndex, int pageSize)
        {
            var url = UrlConfigs.Borrow.GetBorrowsUrl(_borrowUrl, memberId, status, borrowDate, pageIndex, pageSize);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<Borrow>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return await result.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<IPagedList<Borrow>> GetBorrowsByDateAsync(DateTime borrowDate, int pageIndex, int pageSize)
        {
            var url = UrlConfigs.Borrow.GetBorrowsUrl(_borrowUrl, null, null, borrowDate, pageIndex, pageSize);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<Borrow>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return await result.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<IPagedList<Borrow>> GetBorrowsByMemberAsync(string memberId, int pageIndex, int pageSize)
        {
            var url = UrlConfigs.Borrow.GetBorrowsUrl(_borrowUrl, memberId, null, null, pageIndex, pageSize);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<Borrow>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return await result.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<IPagedList<Borrow>> GetBorrowsByStatusAsync(int status, int pageIndex, int pageSize)
        {
            var url = UrlConfigs.Borrow.GetBorrowsUrl(_borrowUrl, null, status, null, pageIndex, pageSize);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<List<Borrow>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return  await result.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<Borrow> GetDraftBorrow(string memberId)
        {
            var url = UrlConfigs.Aggregator.GetDraftBorrow(_aggregatorUrl, memberId);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<Borrow>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }

        public async Task<bool> Pickup(int id)
        {
            var url = UrlConfigs.Borrow.GetPickupUrl(_borrowUrl);
            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };
            var client = _httpClient.CreateClient("MVCClient");
            var content = new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url,content);

            return true;
        }

        public async Task<bool> Return(int id)
        {
            var url = UrlConfigs.Borrow.GetReturnUrl(_borrowUrl);

            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };

            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PutAsync(url, 
                new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json"));

            return true;
        }

        public async Task<bool> SetWaitForPickup(int id)
        {
            var url = UrlConfigs.Borrow.GetWaitForPickupUrl(_borrowUrl);

            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };

            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PutAsync(url, new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json"));

            return true;
        }

        public  async Task<bool> SetWaitForShip(int id)
        {
            var url = UrlConfigs.Borrow.GetWaitForShipUrl(_borrowUrl);

            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };

            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PutAsync(url, new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json"));
            return true;
        }

        public  async Task<bool> Ship(int id)
        {
            var url = UrlConfigs.Borrow.GetShipUrl(_borrowUrl);

            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };

            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PutAsync(url, new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json"));
            return true;
        }

        public async Task<bool> Cancel(int id)
        {
            var url = UrlConfigs.Borrow.GetCancelUrl(_borrowUrl);

            BorrowDto borrow = new BorrowDto
            {
                Id = id
            };

            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.PutAsync(url, new StringContent(JsonSerializer.Serialize(borrow), System.Text.Encoding.UTF8, "application/json"));
            return true;
        }


    }
}
