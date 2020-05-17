using BooksBase.DataAccess;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Infrastructure
{
    public class ClaimService : IClaimService
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IPermissionService _permissionService;

        public ClaimService(DataContext db,
            ICurrentUserAccessor currentUserAccessor,
            IPermissionService permissionService)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _permissionService = permissionService;
        }

        public async Task<List<string>> GetClaimsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = GetClaimsQuery(userId);
            return await query.Select(q => q.ClaimValue)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasClaimAsync(string requirement, CancellationToken cancellationToken = default)
        {
            var claim = _currentUserAccessor.User?.FindFirst(JwtRegisteredClaimNames.Sid);
            if(claim == null)
            {
                return false;
            }

            var query = GetClaimsQuery(new Guid(claim.Value));
            return await query.AnyAsync(q => q.ClaimType == CustomClaimTypes.Permission && q.ClaimValue == requirement, cancellationToken);
        }

        private IQueryable<RoleClaim> GetClaimsQuery(Guid userId)
        {
            return _db.Users.Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                .ThenInclude(role => role.RoleClaims)
                .Where(user => user.Id == userId)
                .SelectMany(user => user.UserRoles.Select(userRole => userRole.Role))
                .SelectMany(role => role.RoleClaims);                
        }
    }
}
