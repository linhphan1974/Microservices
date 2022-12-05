using BookOnline.Aggregator.Models;

namespace BookOnline.Aggregator.Services
{
    public interface IBasketService
    {
        Task<BasketDto> GetBasketByIdAsync(string memberId);
        Task<BasketDto> UpdateBasketAsync(BasketDto basket);
    }
}
