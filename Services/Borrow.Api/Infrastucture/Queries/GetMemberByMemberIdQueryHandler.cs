namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class GetMemberByMemberIdQueryHandler : IRequestHandler<GetMemberByMemberIdQuery, MemberDto>
    {
        private string _connectionString;
        private readonly ILogger<GetMemberByMemberIdQueryHandler> _logger;

        public GetMemberByMemberIdQueryHandler(IOptions<Settings> setting, ILogger<GetMemberByMemberIdQueryHandler> logger)
        {
            _connectionString = setting.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<MemberDto> Handle(GetMemberByMemberIdQuery request, CancellationToken cancellationToken)
        {
            MemberDto dto = new MemberDto();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = await connection.QueryAsync<dynamic>
                        (@"Select * From Member Where MemberId = @Id", new { request.Id });
                    dto = MemberDtoParser.MapToMemberDto(query);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get member fail");
            }

            return dto;
        }
    }
}
