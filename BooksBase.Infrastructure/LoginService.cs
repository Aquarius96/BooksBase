using BooksBase.Models.Auth;
using BooksBase.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BooksBase.Infrastructure
{
    public class LoginService : ILoginService
    {
        private readonly AuthSettings _authSettings;

        public LoginService(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        public UserTokenInfo GetToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Secret));

            var token = new JwtSecurityToken(
                _authSettings.Issuer,
                _authSettings.Audience,
                expires: DateTime.UtcNow.AddHours(_authSettings.ExpirationTimeHours),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var userToken = new UserTokenInfo
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };

            return userToken;
        }
    }
}
