using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using System.Diagnostics;

namespace BookOnline.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewRenderService _viewRenderService;
        private readonly IBasketService _basketService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private ApplicationUser _user;

        public HomeController(ILogger<HomeController> logger, 
            IBasketService basketService,
            IIdentityParser<ApplicationUser> appUserParser,
            IViewRenderService viewRenderService)
        {
            _logger = logger;
            _basketService = basketService;
            _appUserParser = appUserParser;
            _viewRenderService = viewRenderService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}