using System.Security.Principal;

namespace BookOnline.MvcClient.Services
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}
