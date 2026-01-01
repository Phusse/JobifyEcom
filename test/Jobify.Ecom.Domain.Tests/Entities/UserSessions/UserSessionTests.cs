using FluentAssertions;
using Jobify.Ecom.Domain.Entities.UserSessions;

namespace Jobify.Ecom.Domain.Tests.Entities.UserSessions;

public class UserSessionTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Constructor_Should_Create_Session_With_Valid_State()
    {
        TimeSpan sliding = TimeSpan.FromMinutes(30);
        TimeSpan absolute = TimeSpan.FromHours(8);

        DateTime before = DateTime.UtcNow;

        UserSession session = new(UserId, rememberMe: true, sliding, absolute);

        DateTime after = DateTime.UtcNow;

        session.Id.Should().NotBe(Guid.Empty);

        session.UserId.Should().Be(UserId);
        session.RememberMe.Should().BeTrue();

        session.ExpiresAt.Should().BeAfter(before);
        session.ExpiresAt.Should().BeBefore(after.Add(sliding));

        session.AbsoluteExpiresAt.Should().BeAfter(before);
        session.AbsoluteExpiresAt.Should().BeBefore(after.Add(absolute));

        session.IsRevoked.Should().BeFalse();
        session.RevokedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_Throw_When_SlidingLifetime_Is_Invalid(int minutes)
    {
        TimeSpan sliding = TimeSpan.FromMinutes(minutes);
        TimeSpan absolute = TimeSpan.FromHours(1);

        Action act = () => _ = new UserSession(UserId, false, sliding, absolute);

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "slidingLifetime");
    }

    [Fact]
    public void Constructor_Should_Throw_When_AbsoluteLifetime_Is_Less_Than_Sliding()
    {
        TimeSpan sliding = TimeSpan.FromHours(2);
        TimeSpan absolute = TimeSpan.FromMinutes(30);

        Action act = () => _ = new UserSession(UserId, false, sliding, absolute);

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "absoluteLifetime");
    }

    [Fact]
    public void Constructor_Should_Update_Audit_Timestamp()
    {
        TimeSpan sliding = TimeSpan.FromMinutes(15);
        TimeSpan absolute = TimeSpan.FromHours(1);

        UserSession session = new(UserId, false, sliding, absolute);

        session.UpdatedAt.Should().BeOnOrAfter(session.CreatedAt);
    }
}
