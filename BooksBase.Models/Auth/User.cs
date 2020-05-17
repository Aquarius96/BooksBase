using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BooksBase.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
