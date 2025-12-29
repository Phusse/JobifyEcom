using Jobify.Ecom.Domain.Abstractions;
using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Entities.Users;

namespace Jobify.Ecom.Domain.Entities.UserSessions;

public class UserSession : IEntity, IAuditable
{
    internal readonly AuditState AuditState = new();

    private UserSession() { }

    public UserSession(Guid userId, bool rememberMe, TimeSpan slidingLifetime, TimeSpan absoluteLifetime)
    {
        if (slidingLifetime <= TimeSpan.Zero)
            throw new ArgumentException("Sliding lifetime must be positive.", nameof(slidingLifetime));

        if (absoluteLifetime < slidingLifetime)
            throw new ArgumentException(
                "Absolute lifetime must be greater than or equal to sliding lifetime.",
                nameof(absoluteLifetime)
            );

        DateTime now = DateTime.UtcNow;

        UserId = userId;
        RememberMe = rememberMe;
        ExpiresAt = now.Add(slidingLifetime);
        AbsoluteExpiresAt = now.Add(absoluteLifetime);

        AuditState.UpdateAudit();
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public DateTime CreatedAt => AuditState.CreatedAt;
    public DateTime UpdatedAt => AuditState.UpdatedAt;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool RememberMe { get; private set; }

    public Guid SessionStamp { get; private set; } = Guid.NewGuid();
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }
    public DateTime AbsoluteExpiresAt { get; private set; }
}
