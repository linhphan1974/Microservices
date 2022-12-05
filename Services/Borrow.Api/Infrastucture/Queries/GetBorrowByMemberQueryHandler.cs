using BookOnline.Borrowing.Api.Infrastucture.Data;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.Service;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByMemberQueryHandler : IRequestHandler<GetBorrowByMemberQuery, List<BorrowDto>>
    {
        private string _connectionString;
        private readonly ILogger<GetBorrowByIdQueryHandler> _logger;

        public GetBorrowByMemberQueryHandler(IOptions<Settings> setting, ILogger<GetBorrowByIdQueryHandler> logger)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<List<BorrowDto>> Handle(GetBorrowByMemberQuery request, CancellationToken cancellationToken)
        {
            List<BorrowDto> borrows = new List<BorrowDto>();
            int from = ((int)request.PageIndex * request.PageSize) + 1;
            int to = ((int)request.PageIndex + 1) * request.PageSize;
            long totalPage = 0;
            var topStr = "Top(@To)";
            var where = "WHERE RowNumber BETWEEN @From AND @To";
            var queryStr = String.Empty;

            if (request.PageSize == 0)
            {
                topStr = String.Empty;
                where = String.Empty;
            }
                
            queryStr = $"WITH Data AS (SELECT *, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber) FROM Borrow WHERE MemberId = @MemberId " +
                        $"SELECT {topStr} *, SELECT MAX(RowNumber) FROM Data) AS TotalRow FROM Data WHERE " + where;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (queryStr, new { MemberId = request.MemberId, From = from, To = to});

                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            var borrow = BorrowDtoParser.MapToBorrowDto(item);
                            totalPage = query.FirstOrDefault().TotalRow;

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
                _logger.LogError(ex, "Get borrow by member fail");
            }

            return borrows;
        }
    }
}
