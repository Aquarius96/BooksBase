using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BooksBase.Shared
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IClaimService _claimService;

        public PermissionAuthorizationHandler(IClaimService claimService)
        {
            _claimService = claimService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if(await _claimService.HasClaimAsync(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
