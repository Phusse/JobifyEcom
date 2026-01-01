using FluentAssertions;
using Jobify.Ecom.Domain.Components.Auditing;

namespace Jobify.Ecom.Domain.Tests.Components.Auditing;

public class AuditStateTests
{
    [Fact]
    public void Constructor_Should_SetCreatedAtAndUpdatedAtToNow()
    {
        DateTime before = DateTime.UtcNow;

        AuditState auditState = new();
        DateTime after = DateTime.UtcNow;

        auditState.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        auditState.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void UpdateAudit_Should_UpdateUpdatedAtToLaterTime()
    {
        AuditState auditState = new();
        DateTime originalUpdatedAt = auditState.UpdatedAt;

        Thread.Sleep(10);
        auditState.UpdateAudit();

        auditState.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
