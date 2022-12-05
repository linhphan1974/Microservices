namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetAllBorrowQueryHandler : IRequestHandler<GetAllBorrowQuery, List<BorrowDto>>
    {
        private string _connectionString;
        private ILogger<GetAllBorrowQueryHandler> _logger;

        public GetAllBorrowQueryHandler(IOptions<Settings> setting, ILogger<GetAllBorrowQueryHandler> logger)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<List<BorrowDto>> Handle(GetAllBorrowQuery request, CancellationToken cancellationToken)
        {
            List<BorrowDto> result = new List<BorrowDto>();

            int totalPage = 0;
            int from = ((int)request.PageIndex * request.PageSize) + 1;
            int to = ((int)request.PageIndex + 1) * request.PageSize;
            string topStr = "TOP(@To)";
            string whereStr = "WHERE RowNumber BETWEEN @From AND @To ORDER BY RowNumber";

            if (request.PageSize == 0)
            {
                topStr = String.Empty;
                whereStr = String.Empty;
            }

            string queryStr = $"WITH Data AS (SELECT *, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber FROM Borrow)" +
                $"SELECT {topStr} * ,(SELECT MAX(RowNumber) FROM Data) AS TotalRow FROM Data {whereStr}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (queryStr, new {To = to, From = from});

                    totalPage = query.FirstOrDefault()[0].TotalRow;
                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            var itemQueryStr = $"SELECT * FROM BorrowItem WHERE BorrowId = @BorrowId";
                            var itemQuery = await connection.QueryAsync<BorrowItemDto>(itemQueryStr, new {BorrowId = item[0].Id});
                            var borrow = BorrowDtoParser.MapToBorrowDto(query);
                         
                            borrow.Items.AddRange(itemQuery);
                            result.Add(borrow);
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
