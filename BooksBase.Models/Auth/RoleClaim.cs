using Microsoft.AspNetCore.Identity;
using System;

namespace BooksBase.Models.Auth
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual Role Role { get; set; }
    }
}
