using BookOnline.Basket.Api.Models;

namespace BookOnline.Basket.Api.Infrastructure.Data
{
    public interface IBasketRepository
    {
        Task<BookBasket> GetBasketAsync(string memberId);

        Task<BookBasket> UpdateBasketAsync(BookBasket basket);

        Task<bool> DeleteBasketAsync(string memberId);
    }
}
