using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Shared
{
    public interface IClaimService : IService
    {
        Task<bool> HasClaimAsync(string requirement, CancellationToken cancellationToken = default);
        Task<List<string>> GetClaimsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
