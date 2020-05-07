using Microsoft.AspNetCore.Identity;
using System;

namespace BooksBase.Models.Auth
{
    public class UserToken : IdentityUserToken<Guid>
    {
    }
}
