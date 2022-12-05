using BookOnline.Borrowing.Api.Infrastucture.Data;
using BookOnline.Borrowing.Api.Infrastucture.Queries;
using BookOnline.Borrowing.Api.Models;
using GrpcBorrow;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static GrpcBorrow.Borrowing;

namespace BookOnline.Borrowing.Api.GrpcService
{
    public class BorrowGrpcService : BorrowingBase
    {
        public override Task<BorrowResponse> CreateDraftBorrow(BorrowDraftRequest request, ServerCallContext context)
        {
            var items = request.Items.Select(i=> new BorrowItemResponse {BookId = i.BookId, PictureUrl = i.PictureUrl, Title = i.BookName });
            var response = new BorrowResponse();

            response.Items.AddRange(items);

            return Task.FromResult(response);
        }
    }
}
