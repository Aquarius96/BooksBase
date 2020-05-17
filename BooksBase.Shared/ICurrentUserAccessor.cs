using System;
using System.Security.Claims;

namespace BooksBase.Shared
{
    public interface ICurrentUserAccessor : IService
    {
        ClaimsPrincipal User { get; }
        Guid UserId { get; }
    }
}
