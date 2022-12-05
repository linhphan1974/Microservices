using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class IdentityConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("basket","Basket Api"),
                new ApiScope("book","Book Api"),
                new ApiScope("borrow","Borrow Api"),
                new ApiScope("webhook","Webhook Api"),
                new ApiScope("aggregator","BookOnline Api"),
                new ApiScope("signalr", "Notification Api")
            };

        public static IEnumerable<Client> Clients(Dictionary<string,string> clientUrl)
        {
            return new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "webhookmvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { $"{clientUrl["webhookmvc"]}/signin-oidc"},//{ "https://localhost:7234/signin-oidc" },
                    FrontChannelLogoutUri = $"{clientUrl["webhookmvc"]}/signout-oidc",//"https://localhost:7234/signout-oidc",
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { $"{clientUrl["webhookmvc"]}/signout-callback-oidc" },                    // scopes that client has access to
                    AllowOfflineAccess = true,
                    RequirePkce = false,
                    RequireConsent = false,
                    AllowPlainTextPkce = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "webhook"
                    }
                },
                
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {$"{clientUrl["mvcclient"]}/signin-oidc"},//{ "https://localhost:7203/signin-oidc" },
                    FrontChannelLogoutUri = $"{clientUrl["mvcclient"]}/signout-oidc",//"https://localhost:7203/signout-oidc",
                    PostLogoutRedirectUris = {$"{clientUrl["mvcclient"]}/signout-callback-oidc"},//{ "https://localhost:7203/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    RequirePkce = false,
                    RequireConsent = false,
                    AllowPlainTextPkce = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "basket",
                        "book",
                        "borrow",
                        "aggregator",
                        "signalr"
                    }
                }
            };
        }
    }
}
