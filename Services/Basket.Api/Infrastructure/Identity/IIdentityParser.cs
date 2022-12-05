using System.Security.Principal;

namespace BookOnline.Basket.Api.Infrastructure
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}
