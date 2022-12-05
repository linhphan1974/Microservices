using BookOnline.Borrowing.Api.Infrastucture.Data;
using BookOnline.Borrowing.Api.Models;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using BookOnline.Borrowing.Api.Service;
using Microsoft.Extensions.Options;

namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto>
    {
        private readonly ILogger<GetBorrowByIdQueryHandler> _logger;
        private string _connectionString;

        public GetBorrowByIdQueryHandler(IOptions<Settings> setting, ILogger<GetBorrowByIdQueryHandler> logger)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<BorrowDto> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using(var connection =  new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (@"Select b.Id as Id, b.MemberId as MemberId, b.BorrowDate as BorrowDate,b.PickupDate as PickupDate,
                            b.ReturnDate as ReturnDate, b.BorrowStatus as BorrowStatus, b.Description, b.ShipType,
                            bi.Id as ItemId, bi.BookId as BookId, bi.Title as Title,bi.PictureUrl as PuctureUrl, bi.BorrowId as BorrowId
                            FROM [Borrow] b
                            Left Join [BorrowItem] bi ON b.Id = bi.BorrowId
                            WHERE b.Id = @Id
                            ", new {request.Id});

                    if(query.Count() > 0)
                    {
                        var borrow = BorrowDtoParser.MapToBorrowDto(query);
                        return borrow;
                    }
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Get borrow by id fail");
            }
            return null;
        }

    }
}
