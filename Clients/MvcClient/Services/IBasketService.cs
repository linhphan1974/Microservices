using BookOnline.MvcClient.Models;

namespace BookOnline.MvcClient.Services
{
    public interface IBasketService
    {
        Task<Cart> GetCurrentCartAsync(string id);
        Task AddToCartAsync(string id, int bookId);
        Task<bool> CartExist(string id);
    }
}
