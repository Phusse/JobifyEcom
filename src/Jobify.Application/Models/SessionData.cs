using Jobify.Domain.Enums;

namespace Jobify.Application.Models;

public record SessionData(
    Guid SessionId,
    Guid UserId,
    DateTime ExpiresAt,
    DateTime AbsoluteExpiresAt,
    bool IsRevoked,
    bool RememberMe,
    bool IsLocked,
    SystemRole Role
)
{
    public bool IsExpired()
    {
        DateTime now = DateTime.UtcNow;
        return now >= ExpiresAt || now >= AbsoluteExpiresAt || IsRevoked;
    }
}
