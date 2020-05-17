using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BooksBase.Models.Auth
{
    public class Role : IdentityRole<Guid>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims{ get; set; }
    }
}
