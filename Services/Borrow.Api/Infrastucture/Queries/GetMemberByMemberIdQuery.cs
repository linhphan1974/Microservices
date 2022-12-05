namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetMemberByMemberIdQuery : IRequest<MemberDto>
    {
        public string Id { get; set; }

        public GetMemberByMemberIdQuery(string id)
        {
            Id = id;
        }
    }
}
