using BookOnline.Aggregator.Models;
using BookOnline.Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookOnline.Aggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IBookService _bookService;

        public BasketController(IBasketService basketService, IBookService bookService)
        {
            _basketService = basketService;
            _bookService = bookService;
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UpdateBasket([FromBody] UpdateBasketClientRequest request)
        {
            var basket = new BasketDto
            {
                BasketId = request.MemberId,
            };

            basket.Items.AddRange(request.Items.Select(i => new BasketItemDto
            {
                BookCode = i.BookCode,
                BookId = i.BookId,
                BookName = i.BookName,
                PictureUrl = i.PictureUrl,
                Id = i.Id
            }));

            await _basketService.UpdateBasketAsync(basket);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<Cart> GetBasketByIdAsync(string id)
        {
            var basket = await _basketService.GetBasketByIdAsync(id);

            if (basket is not null)
            {
                var result = new Cart
                {
                    Id = id
                };
                basket.Items.ForEach(i =>
                    result.Items.Add(new CartItem
                    {
                        BookCode = i.BookCode,
                        BookId = i.BookId,
                        BookName = i.BookName,
                        Id = i.Id,
                        PictureUrl = i.PictureUrl
                    }));
                return result;
            }

            return new Cart();
        }

        [HttpPost]
        [Route("addtocart")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(418)]
        public async Task<Cart> AddToBasket(AddToBasketRequestClient request)
        {
            BasketDto responseDto = new BasketDto();
            // Step 1 : Get book data
            var book = await _bookService.GetBookByIdAsync(request.BookId);
            // Step 2: Get current basket from cache database
            var currentBasket = await _basketService.GetBasketByIdAsync(request.BasketId);
            if(string.IsNullOrEmpty(currentBasket.BasketId))
            {
                currentBasket = new BasketDto { BasketId = request.BasketId };
            }

            //Step 3: Get book item to check book already add or not
            var bookItem = currentBasket.Items.FirstOrDefault(i => i.BookId == request.BookId);
            //Step 4: Add book item to basket
            if (bookItem == null)
            {
                currentBasket.Items.Add(new BasketItemDto
                {
                    BookCode = book.ISBN,
                    BookId = book.Id,
                    BookName = book.Title,
                    PictureUrl = book.PictureUrl,
                    Id = Guid.NewGuid().ToString()
                });
            }
            //Step 5: Update basket
            responseDto = await _basketService.UpdateBasketAsync(currentBasket);

            var basketReponse = new Cart
            {
                Id = responseDto.BasketId
            };

            basketReponse.Items.AddRange(responseDto.Items.Select(i =>
                new CartItem
                {
                    PictureUrl = i.PictureUrl,
                    Id = i.Id,
                    BookCode = i.BookCode,
                    BookId = i.BookId,
                    BookName = i.BookName
                }));

            return basketReponse;

        }

    }
}
