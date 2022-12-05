using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly IBookService _bookService;
        private readonly IViewRenderService _viewRenderService;
        public CartController(IBasketService basketService, 
            IIdentityParser<ApplicationUser> appUserParser,
            IBookService bookService,
            IViewRenderService viewRenderService)
        {
            _basketService = basketService;
            _appUserParser = appUserParser;
            _bookService = bookService;
            _viewRenderService = viewRenderService;
        }
        public IActionResult Index()
        {
            //var user = _appUserParser.Parse(HttpContext.User);
            //var cart = await _basketService.GetCurrentCartAsync(user.Id);
            //if (cart == null)
            //{
            //    return NotFound();
            //}

            return View();
        }


        public async Task<IActionResult> AddToCartAjax(int bookId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            await _basketService.AddToCartAsync(user.Id, bookId);

            return Json(new { Success = true, Message = "Add to cart successed" });
        }
        [HttpGet]
        public async Task<JsonResult> GetCurrentCartAjax()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var currentBasket = await _basketService.GetCurrentCartAsync(user.Id);
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_CurrentCartPartial", currentBasket) });
            }
            else
            {
                return Json(new { Message = "Login is required" });
            }
        }

    }
}
