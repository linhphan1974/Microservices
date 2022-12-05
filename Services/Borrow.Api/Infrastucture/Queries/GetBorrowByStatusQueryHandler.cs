
namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByStatusQueryHandler : IRequestHandler<GetBorrowByStatusQuery, List<BorrowDto>>
    {
        private string _connectionString;
        private readonly ILogger<GetBorrowByIdQueryHandler> _logger;
        public GetBorrowByStatusQueryHandler(ILogger<GetBorrowByIdQueryHandler> logger,IOptions<Settings> setting)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<List<BorrowDto>> Handle(GetBorrowByStatusQuery request, CancellationToken cancellationToken)
        {
            List<BorrowDto> borrows = new List<BorrowDto>();
            int from = ((int)request.PageIndex * request.PageSize) + 1;
            int to = ((int)request.PageIndex + 1) * request.PageSize;
            long totalPage = 0;
            string topStr = "TOP(@To)";
            string whereStr = "WHERE RowNumber BETWEEN @From AND @To ORDER BY RowNumber";

            if (request.PageSize == 0)
            {
                topStr = String.Empty;
                whereStr = String.Empty;
            }

            string queryStr = $"WITH Data AS (SELECT *, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber FROM Borrow WHERE BorrowStatus = @Status)" +
                $"SELECT {topStr} * ,(SELECT MAX(RowNumber) FROM Data) AS TotalRow FROM Data {whereStr}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (queryStr, new { Status = request.Status, From = from, To = to });

                    totalPage = query.FirstOrDefault().TotalRow;

                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            var borrow = BorrowDtoParser.MapToBorrowDto(item);

                            if (borrow != null)
                            {
                                var itemQueryStr = $"SELECT * FROM BorrowItem WHERE BorrowId = @Id";
                                var items = await connection.QueryAsync<BorrowItemDto>(itemQueryStr, new { Id = borrow.Id });

                                borrow.Items.AddRange(items);
                                borrows.Add(borrow);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get borrow by status fail");
            }

            return borrows;
        }
    }
}
