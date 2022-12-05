using BookOnline.Basket.Api.Infrastructure;
using BookOnline.Basket.Api.Infrastructure.Data;
using BookOnline.Basket.Api.Infrastructure.Events;
using BookOnline.Basket.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQEventBus;
using System.Security.Claims;

namespace BookOnline.Basket.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IIdentityParser<ApplicationUser> _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;


        public BasketController(IBasketRepository basketRepository,
            IIdentityParser<ApplicationUser> identityService,
            IEventBus eventBus,
            ILogger<BasketController> logger)
        {
            _eventBus = eventBus;
            _identityService = identityService;
            _logger = logger;
            _basketRepository = basketRepository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasketAsync(string Id)
        {
            var response = await _basketRepository.GetBasketAsync(Id);

            return Ok(response ?? new BookBasket(Id));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBasketAsync([FromBody]BookBasket basket)
        {
            return Ok(await _basketRepository.UpdateBasketAsync(basket));

        }

        [HttpDelete("{id}")]
        public async Task DeleteBasketAsync(string Id)
        {
            await _basketRepository.DeleteBasketAsync(Id);
        }


        [Route("checkout")]
        [HttpPost]
        public async Task<IActionResult> CheckOutAsync(CheckoutRequest request)
        {
            try
            {
                // Create draft order.
                var user = _identityService.Parse(HttpContext.User);
                BookBasket basket = await _basketRepository.GetBasketAsync(user.Id);

                if (basket == null)
                {
                    return BadRequest();
                }

                var username = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

                CheckoutBorrowEvent checkoutBorrowEvent = new CheckoutBorrowEvent(request.ShipType, basket, user);
                _eventBus.Publish(checkoutBorrowEvent);
            }
            catch(Exception ex)
            {

            }
            return Ok(true);
        }
    }
}
