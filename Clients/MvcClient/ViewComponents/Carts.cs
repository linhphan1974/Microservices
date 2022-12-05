using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.ViewComponents
{
    public class Carts : ViewComponent
    {
        private readonly IBasketService _service;
        public Carts(IBasketService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var mv = new Cart();
            try
            {
                mv = await _service.GetCurrentCartAsync(userId);
            }
            catch(Exception ex)
            {
                ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative, please try later on. ({ex.GetType().Name} - {ex.Message}))";
            }

            return View(mv);
        }
    }
}
