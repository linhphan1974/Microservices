using BookOnline.Borrowing.Api.Infrastucture.Queries;

namespace BookOnline.Borrowing.Api.Service
{
    public static class MemberDtoParser
    {
        public static MemberDto MapToMemberDto(dynamic result)
        {
            MemberDto memberDto = new MemberDto
            {
                Id = result[0].Id,
                MemberId = result[0].MemberId,
                MemberName = result[0].MemberName,
                Status = result[0].Status
            };

            return memberDto;
        }
    }
}
