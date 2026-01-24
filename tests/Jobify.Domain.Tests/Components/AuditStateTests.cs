using FluentAssertions;
using Jobify.Domain.Components.Auditing;
using Xunit;

namespace Jobify.Domain.Tests.Components;

public class AuditStateTests
{
    [Fact]
    public void Constructor_SetsCreatedAtToNow()
    {
        // Arrange & Act
        var auditState = new AuditState();

        // Assert
        auditState.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        auditState.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreatedAt_DoesNotChangeAfterConstruction()
    {
        // Arrange
        var auditState = new AuditState();
        var originalCreatedAt = auditState.CreatedAt;
        Thread.Sleep(10);

        // Assert
        auditState.CreatedAt.Should().Be(originalCreatedAt);
    }
}
