using BookOnline.Borrowing.Api.Infrastucture.Queries;

namespace BookOnline.Borrowing.Api.Service
{
    public static class BorrowDtoParser
    {
        public static BorrowDto MapToBorrowDto(dynamic result)
        {
            BorrowDto borrowDto = new BorrowDto
            {
                Id = result.Id,
                BorrowDate = result.BorrowDate,
                BorrowStatus = result.BorrowStatus,
                MemberId = result.MemberId,
                PickupDate = result.PickupDate,
                ReturnDate = result.ReturnDate,
                ShipType = result.ShipType,
                Description = result.Description,
                Items = new List<BorrowItemDto>()
            };

            return borrowDto;
        }

    }
}
