using System;

namespace BooksBase.Shared
{
    public class UserTokenInfo
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
