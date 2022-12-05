using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
