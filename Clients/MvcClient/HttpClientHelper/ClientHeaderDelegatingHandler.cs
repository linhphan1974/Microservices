using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;

namespace BookOnline.MvcClient.HttpClientHelper
{
    public class ClientHeaderDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
       // public readonly IOptions<IdentityServerSettings> identityServerSettings;
        //public readonly DiscoveryDocumentResponse discoveryDocument;
        private readonly HttpClient _httpClient;


        public ClientHeaderDelegatingHandler(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor ?? 
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            //discoveryDocument = httpClient.GetDiscoveryDocumentAsync("https://host.docker.internal:8012").Result;

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var authorizationHeader = _httpContextAccessor.HttpContext
                    .Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
                }

                var token = await GetToken();
                if(token is not null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch(Exception ex)
            {

            }
            return await base.SendAsync(request, cancellationToken);
        }

        async Task<string> GetToken()
        {
            const string ACCESS_TOKEN = "access_token";

            return await _httpContextAccessor.HttpContext
                .GetTokenAsync(ACCESS_TOKEN);
        }
    }
}
