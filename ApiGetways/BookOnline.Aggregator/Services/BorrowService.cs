using BookOnline.Aggregator.Models;
using GrpcBorrow;

namespace BookOnline.Aggregator.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly Borrowing.BorrowingClient _client;
        private readonly ILogger<BorrowService> _logger;

        public BorrowService(Borrowing.BorrowingClient client, ILogger<BorrowService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<BorrowDto> CreateDraftBorrow(BasketDto basket)
        {
            var draft = new BorrowDraftRequest();
            draft.Items.AddRange(basket.Items.Select(i => new BasketItem { BookId = i.BookId, PictureUrl = i.PictureUrl, BookName = i.BookName }).ToList());
            
            var response = await _client.CreateDraftBorrowAsync(draft);

            var dto = new BorrowDto();

            dto.Items.AddRange(response.Items.Select(i => new BorrowItemDto { BookId = i.BookId, PictureUrl = i.PictureUrl, Title = i.Title }));

            _logger.LogTrace("Create draft borrow via grpc service");
            return dto;
        }
    }
}
