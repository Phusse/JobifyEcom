namespace Jobify.Ecom.Domain.Components.Auditing;

public class AuditState
{
    private AuditState() { }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public void UpdateAudit() => UpdatedAt = DateTime.UtcNow;
}
