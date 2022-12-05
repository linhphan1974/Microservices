using AutoMapper;
using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using BookOnline.MvcClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookOnline.MvcClient.Controllers
{
    //[Authorize(Roles = "Regular")]
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        private readonly IOptions<AppSettings> _setting;
        private readonly IMapper _mapper;
        public BorrowController(IBorrowService borrowService, 
            IIdentityParser<ApplicationUser> identityParser, 
            IOptions<AppSettings> setting, IMapper mapper)
        {
            _borrowService = borrowService;
            _identityParser = identityParser;
            _setting = setting;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var user = _identityParser.Parse(HttpContext.User);
            var result = await _borrowService.GetBorrowsByMemberAsync(user.Id, 0, _setting.Value.DefaultPageSize);

            BorrowViewModel model = new BorrowViewModel
            {
                Data = result.ToList(),
                PageSize = result.Count,
                Count = result.TotalItemCount,
                PageIndex = result.PageNumber,
                TotalPage = result.PageCount
            };


            return View(model);
        }

        public async Task<JsonResult> GetCurrentBorrowCount()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Regular"))
            {
                var user = _identityParser.Parse(HttpContext.User);
                var model = await _borrowService.GetActiveBorrowsAsync(user.Id);

                return Json(new { count = model.Count });
            }
            else
                return Json(new { count = 0 });
        }

        public async Task<IActionResult> Create()
        {
            var user = _identityParser.Parse(HttpContext.User);
            var borrow = await _borrowService.GetDraftBorrow(user.Id);
            
            return View(borrow);
        }

        public async Task<IActionResult> Checkout(Borrow model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid borrow";
                return View();
            }

            var result = await _borrowService.Checkout(model);
            if (result)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Error", "Home");
        }
    }
}
