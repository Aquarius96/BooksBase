using BooksBase.Models;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BooksBase.DataAccess
{
    public class DataContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(b =>
            {
                b.HasKey(r => new { r.RoleId, r.UserId });
                b.HasOne(r => r.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(r => r.RoleId);
                b.HasOne(r => r.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(r => r.UserId);
            });
            builder.Entity<RoleClaim>()
                .HasOne(c => c.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(c => c.RoleId);
        }

        public void SeedDatabase(IPermissionService permissionService)
        {
            AddAdmin(permissionService);
            AddUser(permissionService);

            SaveChanges();
        }

        private void AddAdmin(IPermissionService permissionService)
        {
            var adminRole = Roles.FirstOrDefault(r => r.Name == "Admin");
            if(adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                var entry = Add(adminRole);
                adminRole = entry.Entity;
            }

            foreach(var permission in permissionService.GetAllPermissions())
            {
                var displayClaim = RoleClaims.FirstOrDefault(c => c.Role == adminRole && c.ClaimValue == permission.Claim);
                if(displayClaim == null)
                {
                    displayClaim = new RoleClaim
                    {
                        Role = adminRole,
                        ClaimType = "permission",
                        ClaimValue = permission.Claim
                    };
                    Add(displayClaim);
                }
            }

            var admin = Users.FirstOrDefault(u => u.UserName == "admin@booksbase.com");
            if(admin == null)
            {
                admin = new User
                {
                    UserName = "admin@booksbase.com",
                    NormalizedUserName = "ADMIN@BOOKSBASE.COM",
                    Email = "admin@booksbase.com",
                    NormalizedEmail = "ADMIN@BOOKSBASE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEKMfB8DBmA9SiF42UN/d2IXF56sm/XWQimCho8SLqctQrlPdcjXOZzMn5x+tfvXsbA==", //admin
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };
                Add(admin);
            }

            var adminRoleAssignment = UserRoles.FirstOrDefault(r => r.Role == adminRole && r.User == admin);
            if(adminRoleAssignment == null)
            {
                adminRoleAssignment = new UserRole
                {
                    User = admin,
                    Role = adminRole
                };
                Add(adminRoleAssignment);
            }
        }

        private void AddUser(IPermissionService permissionService)
        {
            var userRole = Roles.FirstOrDefault(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Role
                {
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                var entry = Add(userRole);
                userRole = entry.Entity;
            }

            var current = permissionService.GetUserPermissions()
                .Select(permission => permission.Claim)
                .ToList();
            var applied = RoleClaims.Where(claim => claim.Role == userRole)
                .Select(claim => claim.ClaimValue)
                .ToList();
            var toAdd = current.Except(applied);

            foreach(var claim in toAdd)
            {
                var displayClaim = new RoleClaim
                {
                    Role = userRole,
                    ClaimType = "permission",
                    ClaimValue = claim
                };
                Add(displayClaim);
            }

            var toRemove = applied.Except(current);
            RoleClaims.Where(claim => claim.Role == userRole && toRemove.Contains(claim.ClaimValue))
                .DeleteFromQuery();
        }
    }
}
