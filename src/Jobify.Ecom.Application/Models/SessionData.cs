using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Application.Models;

public record SessionData(
    Guid SessionId,
    Guid UserId,
    DateTime ExpiresAt,
    DateTime AbsoluteExpiresAt,
    bool IsRevoked,
    bool RememberMe,
    SystemRole Role
)
{
    public bool IsActive
    {
        get
        {
            DateTime now = DateTime.UtcNow;
            return !IsRevoked && now < ExpiresAt && now < AbsoluteExpiresAt;
        }
    }
}
