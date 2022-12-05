using X.PagedList;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowsQueryByExpressionHandler : IRequestHandler<GetBorrowsQueryByExpression, List<BorrowDto>>
    {
        private string _connectionString;
        private readonly ILogger<GetBorrowsQueryByExpressionHandler> _logger;

        public GetBorrowsQueryByExpressionHandler(IOptions<Settings> setting, ILogger<GetBorrowsQueryByExpressionHandler> logger)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<List<BorrowDto>> Handle(GetBorrowsQueryByExpression request, CancellationToken cancellationToken)
        {
            List<BorrowDto> borrows = new List<BorrowDto>();
            int from = (request.PageIndex * request.PageSize) - (request.PageSize - 1);
            int to = (request.PageIndex) * request.PageSize;
            long totalPage = 0;
            string topStr = "TOP(@To)";
            string whereStr = "WHERE RowNumber BETWEEN @From AND @To";

            if(request.PageSize == 0)
            {
                topStr = string.Empty;
                whereStr = string.Empty;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var where = "WHERE";

                    if(request.Status == null && request.MemberId == null && request.BorrowDate == null)
                        where = string.Empty;

                    if (request.Status.HasValue)
                        where += !string.IsNullOrEmpty(where) ? " BorrowStatus = @Status AND" : "" ;
                    if (request.MemberId.HasValue)
                        where += !string.IsNullOrEmpty(where) ? " MemberId = @MemberId AND" : "";
                    if (request.BorrowDate.HasValue)
                        where += !string.IsNullOrEmpty(where) ? " BorrowDate BETWEEN @BorrowDate AND GETDATE() AND" : "";

                    if (where.EndsWith("AND"))
                        where = where.Remove(where.Length - 3, 3);

                    string queryStr = $"WITH Data AS (SELECT *, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber FROM Borrow {where})" +
                        $"SELECT {topStr} *, (SELECT MAX(RowNumber) FROM Data) AS TotalRow FROM Data {whereStr}";

                    var query = await connection.QueryAsync<dynamic>(
                        queryStr, new { request.Status, request.MemberId, request.BorrowDate, From = from, To = to });

                    totalPage = query.FirstOrDefault().TotalRow;

                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            var borrow = BorrowDtoParser.MapToBorrowDto(item);
                            var itemQuery = await connection.QueryAsync<BorrowItemDto>(
                                $"Select * From BorrowItem Where BorrowId = @Id", new { item.Id });
                            
                            borrow.Items.AddRange(itemQuery);
                            borrows.Add(borrow);
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
