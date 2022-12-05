using BookOnline.Aggregator.Models;
using BookOnline.Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.Aggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IBorrowService _borrowService;

        public BorrowController(IBasketService basketService, IBorrowService borrowService)
        {
            _basketService = basketService;
            _borrowService = borrowService;
        }

        [HttpGet]
        [Route("{memberId}")]
        public async Task<BorrowDto> CreateDraftBorrow(string memberId)
        {
            var basket = await _basketService.GetBasketByIdAsync(memberId);
            BorrowDto borrow = new BorrowDto();

            if(basket != null)
            {
                borrow = await _borrowService.CreateDraftBorrow(basket);
            }

            return borrow;
        }
    }
}
