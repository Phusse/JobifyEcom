using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Entities.Users;

namespace Jobify.Ecom.Domain.Entities.UserSessions;

public class UserSession
{
    private readonly AuditState _audit = new();

    public static string Audit => nameof(_audit);

    private UserSession() { }

    public UserSession(Guid userId, bool rememberMe, TimeSpan initialLifetime, TimeSpan absoluteLifetime)
    {
        if (absoluteLifetime < initialLifetime)
            throw new InvalidOperationException("Absolute lifetime must be greater than or equal to initial lifetime.");

        DateTime now = DateTime.UtcNow;

        UserId = userId;
        RememberMe = rememberMe;
        ExpiresAt = now.Add(initialLifetime);
        AbsoluteExpiresAt = now.Add(absoluteLifetime);

        _audit.UpdateAudit();
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid SessionStamp { get; private set; } = Guid.NewGuid();

    public bool RememberMe { get; private set; } = false;
    public bool IsRevoked { get; private set; } = false;
    public DateTime? RevokedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }
    public DateTime AbsoluteExpiresAt { get; private set; }

    public DateTime CreatedAt => _audit.CreatedAt;
    public DateTime UpdatedAt => _audit.UpdatedAt;

    public bool IsActive
    {
        get
        {
            DateTime now = DateTime.UtcNow;
            return !IsRevoked && now < ExpiresAt && now < AbsoluteExpiresAt;
        }
    }

    public void ExtendSession(DateTime newExpiry)
    {
        if (newExpiry > AbsoluteExpiresAt)
            newExpiry = AbsoluteExpiresAt;

        if (newExpiry <= ExpiresAt) return;

        ExpiresAt = newExpiry;
        _audit.UpdateAudit();
    }

    public void RefreshSessionStamp()
    {
        SessionStamp = Guid.NewGuid();
        _audit.UpdateAudit();
    }

    public void RevokeAndRotate()
    {
        if (IsRevoked) return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        SessionStamp = Guid.NewGuid();
        _audit.UpdateAudit();
    }
}
