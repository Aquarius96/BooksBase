using BooksBase.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace BooksBase.Infrastructure
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal User => _contextAccessor.HttpContext.User;

        public Guid UserId => Guid.Parse(_contextAccessor.HttpContext.User.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sid).Value);
    }
}
