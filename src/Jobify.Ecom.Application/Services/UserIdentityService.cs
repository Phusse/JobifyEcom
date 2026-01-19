using Jobify.Ecom.Application.Constants.Cache;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Services;

public class UserIdentityService(AppDbContext db, ICacheService cacheService)
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public async Task<UserIdentity?> GetBySourceUserIdAsync(Guid sourceUserId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CacheKey(sourceUserId);
        UserIdentity? cachedData = await cacheService.GetAsync<UserIdentity>(cacheKey);

        if (cachedData is not null) return cachedData;

        Guid userId = await db.Users
            .Where(u => u.SourceUserId == sourceUserId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == Guid.Empty) return null;

        UserIdentity identity = new(userId);

        await cacheService.SetAsync(cacheKey, identity, CacheTtl);

        return identity;
    }

    public async Task InvalidateBySourceUserIdAsync(Guid sourceUserId)
        => await cacheService.RemoveAsync(CacheKey(sourceUserId));

    private static string CacheKey(Guid sourceUserId) => $"{CacheKeys.UserIdentity}{sourceUserId:N}";
}
