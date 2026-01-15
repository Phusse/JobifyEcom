namespace Jobify.Ecom.Domain.Components.Auditing;

public class AuditState
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    internal void UpdateAudit() => UpdatedAt = DateTime.UtcNow;
}
