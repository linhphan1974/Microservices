using BookOnline.Basket.Api.Infrastructure.Data;
using BookOnline.Basket.Api.Models;
using Grpc.Core;
using GrpcBasket;
using static GrpcBasket.Basket;

namespace BookOnline.Basket.Api.GrpcService
{
    public class BasketGrpcService : BasketBase
    {
        private readonly IBasketRepository _repository;
        private ILogger<BasketGrpcService> _logger;
        public BasketGrpcService(IBasketRepository repository, ILogger<BasketGrpcService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override async Task<BasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Get basket by id via grpc service");

            var basket = await _repository.GetBasketAsync(request.BasketId);

            if (basket is not null)
            {
                var response = new BasketResponse
                {
                    BasketId = basket.MemberId,
                };

                response.Items.AddRange(basket.Items.Select(i =>
                    new BasketItemResponse
                    {
                        BookCode = i.BookCode,
                        BookId = i.BookId,
                        BookName = i.BookName,
                        Id = i.Id,
                        PictureUrl = i.PictureUrl
                    }));

                context.Status = new Status(StatusCode.OK, $"Basket with id {request.BasketId} do exist");
                return response;
            }

            return new BasketResponse();
        }

        public async override Task<BasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Update basket via grpc service");
            BookBasket basket = new BookBasket(request.BasketId);
            basket.Items.AddRange(request.Items.Select(i => new BasketItem { BookCode = i.BookCode, BookId = i.BookId, BookName = i.BookName, Id = i.Id, PictureUrl = i.PictureUrl }));

            var basketUpdated = await _repository.UpdateBasketAsync(basket);

            if (basketUpdated != null)
            {
                var response = new BasketResponse { BasketId = basketUpdated.MemberId };
                response.Items.AddRange(basketUpdated.Items.Select(i =>
                        new BasketItemResponse
                        {
                            BookCode = i.BookCode,
                            Id = i.Id,
                            BookId = i.BookId,
                            BookName = i.BookName,
                            PictureUrl = i.PictureUrl
                        }));
                return response;
            }
            context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.BasketId} do not exist");

            return null;
        }
    }
}
