using FluentAssertions;
using Jobify.Domain.Entities.UserSessions;
using Xunit;

namespace Jobify.Domain.Tests.Entities.UserSessions;

public class UserSessionTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly TimeSpan _validSlidingLifetime = TimeSpan.FromHours(2);
    private readonly TimeSpan _validAbsoluteLifetime = TimeSpan.FromDays(7);

    [Fact]
    public void Constructor_WithValidData_CreatesSession()
    {
        // Arrange & Act
        var session = new UserSession(_userId, rememberMe: true, _validSlidingLifetime, _validAbsoluteLifetime);

        // Assert
        session.UserId.Should().Be(_userId);
        session.RememberMe.Should().BeTrue();
        session.Id.Should().NotBeEmpty();
        session.IsRevoked.Should().BeFalse();
        session.RevokedAt.Should().BeNull();
        session.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.Add(_validSlidingLifetime), TimeSpan.FromSeconds(1));
        session.AbsoluteExpiresAt.Should().BeCloseTo(DateTime.UtcNow.Add(_validAbsoluteLifetime), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithZeroSlidingLifetime_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new UserSession(_userId, true, TimeSpan.Zero, _validAbsoluteLifetime);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Sliding lifetime must be positive*");
    }

    [Fact]
    public void Constructor_WithNegativeSlidingLifetime_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new UserSession(_userId, true, TimeSpan.FromHours(-1), _validAbsoluteLifetime);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Sliding lifetime must be positive*");
    }

    [Fact]
    public void Constructor_WithAbsoluteLessThanSliding_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new UserSession(_userId, true, TimeSpan.FromDays(7), TimeSpan.FromHours(2));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Absolute lifetime must be greater than or equal to sliding lifetime*");
    }

    [Fact]
    public void IsExpired_WhenExpiresAtPassed_ReturnsTrue()
    {
        // Arrange - create a session with very short lifetime
        var session = new UserSession(_userId, false, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2));
        Thread.Sleep(10); // Wait for it to expire

        // Act
        var result = session.IsExpired();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenRevoked_ReturnsTrue()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        session.Revoke();

        // Act
        var result = session.IsExpired();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenValid_ReturnsFalse()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);

        // Act
        var result = session.IsExpired();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void ShouldRefresh_WithInvalidPercent_ThrowsArgumentOutOfRangeException(double percent)
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);

        // Act
        var act = () => session.ShouldRefresh(percent);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*between 1 and 100*");
    }

    [Fact]
    public void ShouldRefresh_WhenRevoked_ReturnsFalse()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        session.Revoke();

        // Act
        var result = session.ShouldRefresh(50);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldRefresh_WhenFreshSession_ReturnsFalse()
    {
        // Arrange - session with long lifetime, just created
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);

        // Act - threshold 50% means refresh when 50% time has passed
        var result = session.ShouldRefresh(50);

        // Assert - session is fresh, should not need refresh
        result.Should().BeFalse();
    }

    [Fact]
    public void ExtendSession_WhenRevoked_DoesNothing()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        session.Revoke();
        var originalExpiresAt = session.ExpiresAt;

        // Act
        session.ExtendSession(TimeSpan.FromHours(1));

        // Assert
        session.ExpiresAt.Should().Be(originalExpiresAt);
    }

    [Fact]
    public void ExtendSession_CapsAtAbsoluteExpiry()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        var absoluteExpiry = session.AbsoluteExpiresAt;

        // Act - try to extend beyond absolute
        session.ExtendSession(TimeSpan.FromDays(30));

        // Assert
        session.ExpiresAt.Should().Be(absoluteExpiry);
    }

    [Fact]
    public void ExtendSession_WithValidExtension_UpdatesExpiresAt()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        var originalUpdatedAt = session.UpdatedAt;
        Thread.Sleep(10);

        // Act
        session.ExtendSession(TimeSpan.FromHours(1));

        // Assert
        session.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Revoke_SetsIsRevokedTrue()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);

        // Act
        session.Revoke();

        // Assert
        session.IsRevoked.Should().BeTrue();
        session.RevokedAt.Should().NotBeNull();
        session.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_DoesNotUpdateTimestamp()
    {
        // Arrange
        var session = new UserSession(_userId, true, _validSlidingLifetime, _validAbsoluteLifetime);
        session.Revoke();
        var firstRevokedAt = session.RevokedAt;
        Thread.Sleep(10);

        // Act
        session.Revoke();

        // Assert
        session.RevokedAt.Should().Be(firstRevokedAt);
    }
}
