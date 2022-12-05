using System.Security.Principal;

namespace BookOnline.Borrowing.Api.Service;

public interface IIdentityParser<T>
{
    T Parse(IPrincipal principal);
}
