using BooksBase.Models.Auth;

namespace BooksBase.Shared
{
    public interface ILoginService : IService
    {
        UserTokenInfo GetToken(User user);
    }
}
