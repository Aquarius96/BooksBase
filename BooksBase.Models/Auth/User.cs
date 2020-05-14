using Microsoft.AspNetCore.Identity;
using System;

namespace BooksBase.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
