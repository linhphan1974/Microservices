namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetAvailableBorrowQueryHandler : IRequestHandler<GetAvailableBorrowQuery, List<BorrowDto>>
    {
        private string _connectionString;
        private ILogger<GetAvailableBorrowQueryHandler> _logger;
        public GetAvailableBorrowQueryHandler(ILogger<GetAvailableBorrowQueryHandler> logger, IOptions<Settings> setting)
        {
            _connectionString = setting.Value.ConnectionString; 
            _logger = logger;
        }

        public async Task<List<BorrowDto>> Handle(GetAvailableBorrowQuery request, CancellationToken cancellationToken)
        {
            List<BorrowDto> result = new List<BorrowDto>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (@"Select b.Id as Id, b.MemberId as MemberId, b.BorrowDate as BorrowDate,b.PickupDate as PickupDate,
                            b.ReturnDate as ReturnDate, b.BorrowStatus as BorrowStatus, b.ShipType, b.Description,
                            bi.Id as ItemId, bi.BookId as BookId, bi.Title as Title,bi.PictureUrl as PuctureUrl, bi.BorrowId as BorrowId
                            FROM [Borrow] b
                            Left Join [BorrowItem] bi ON b.Id = bi.BorrowId
                            WHERE b.MemberId = @MemberId AND (BorrowStatus = @Status OR BorrowStatus = @Status1)
                            ", new {request.MemberId, Status = BorrowStatus.WaitForPickup, Status1 = BorrowStatus.WaitForShip});
                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            result.Add(BorrowDtoParser.MapToBorrowDto(item));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all borrow fail");
            }
            return result;
        }
    }
}
