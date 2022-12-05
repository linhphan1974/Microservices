using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.ViewComponents
{

    public class CartCheckout : ViewComponent
    {
        private readonly IBorrowService _borrowService;
        public CartCheckout(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            Borrow borrow = new Borrow();
            borrow = await _borrowService.GetDraftBorrow(userId);

            return View(borrow);
        }
    }
}
