using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.ViewModels;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookOnline.MvcClient.Services
{
    public class BookService : IBookService
    {
        private readonly ILogger<BookService> _logger;
        private readonly IHttpClientFactory _httpClient;
        private readonly string _bookBaseUri;
        private readonly IOptions<AppSettings> _appSettings;

        public BookService(ILogger<BookService> logger, IHttpClientFactory httpClient, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _appSettings = settings;
            _bookBaseUri = $"{_appSettings.Value.BookUrl}/api/book/";
        }

        public async Task<BookItemDto> GetBookByIdAsync(int id)
        {
            string url = UrlConfigs.Book.GetBookByIdUrl(_bookBaseUri, id);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var book = JsonSerializer.Deserialize<BookItemDto>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return book;
        }

        public async Task<BookCatalog> GetBookCatalogByIdAsync(int id)
        {
            string url = UrlConfigs.Book.GetBookCatalogUrl(_bookBaseUri, id);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var catalog = JsonSerializer.Deserialize<BookCatalog>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return catalog;
        }

        public async Task<List<BookCatalog>> GetBookCatalogsAsync()
        {
            string url = UrlConfigs.Book.GetBookCatalogsUrl(_bookBaseUri);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var catalogs = JsonSerializer.Deserialize<List<BookCatalog>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return catalogs;
        }

        public async Task<BookResponseViewModel> GetBooksAsync(int pageIndex, int pageSize, int? bookTypeId = 1, int? catalogId = 1, bool? byAvailable = true)
        {
            try
            {
                string url = UrlConfigs.Book.GetBooksByTypeAndCatalogUrl(_bookBaseUri, pageIndex, pageSize, bookTypeId, catalogId, true);
                var client = _httpClient.CreateClient("MVCClient");
                var response = await client.GetStringAsync(url);

                var result = JsonSerializer.Deserialize<BookResponseViewModel>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch(Exception ex)
            {

            }

            return null;
        }

        public async Task<BookType> GetBookTypeByIdAsync(int id)
        {
            string url = UrlConfigs.Book.GetBookTypeUrl(_bookBaseUri, id);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var bookType = JsonSerializer.Deserialize<BookType>(response, new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true 
            });

            return bookType;
        }

        public async Task<List<BookType>> GetBookTypesAsync()
        {
            string url = UrlConfigs.Book.GetBookTypesUrl(_bookBaseUri);
            var client = _httpClient.CreateClient("MVCClient");
            var response = await client.GetStringAsync(url);

            var bookType = JsonSerializer.Deserialize<List<BookType>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return bookType;
        }
    }
}
