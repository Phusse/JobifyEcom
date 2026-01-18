using Jobify.Domain.Abstractions;
using Jobify.Domain.Components.Auditing;
using Jobify.Domain.Entities.Users;

namespace Jobify.Domain.Entities.UserSessions;

public class UserSession : IEntity, IAuditable
{
    public readonly AuditState AuditState = new();

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

    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }
    public DateTime AbsoluteExpiresAt { get; private set; }

    public bool IsExpired()
    {
        DateTime now = DateTime.UtcNow;
        return now >= ExpiresAt || now >= AbsoluteExpiresAt || IsRevoked;
    }

    public bool ShouldRefresh(double thresholdPercent)
    {
        if (thresholdPercent < 1 || thresholdPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(thresholdPercent), "Threshold percent must be between 1 and 100.");

        double thresholdFraction = thresholdPercent / 100.0;

        if (IsRevoked) return false;

        DateTime now = DateTime.UtcNow;

        if (now >= AbsoluteExpiresAt) return false;

        TimeSpan remaining = ExpiresAt - now;
        TimeSpan total = ExpiresAt - CreatedAt;

        if (total <= TimeSpan.Zero) return true;

        double remainingPercent = remaining.TotalSeconds / total.TotalSeconds;
        return remainingPercent <= thresholdFraction;
    }

    public void ExtendSession(TimeSpan slidingLifetime)
    {
        if (IsRevoked) return;

        DateTime now = DateTime.UtcNow;
        ExpiresAt = now.Add(slidingLifetime);

        if (ExpiresAt > AbsoluteExpiresAt)
            ExpiresAt = AbsoluteExpiresAt;

        AuditState.UpdateAudit();
    }

    public void Revoke()
    {
        if (IsRevoked) return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        AuditState.UpdateAudit();
    }
}
