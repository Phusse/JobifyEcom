using FluentAssertions;
using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Domain.Enums;

namespace Jobify.Ecom.Domain.Tests.Entities.Users;

public class UserTests
{
    private static User CreateValidUser() => new("john_doe", "email_hash", "password_hash");
    private static UserSensitive CreateValidUserSensitive() => UserSensitive.Create("John", "Smith", "Doe", "john.doe@example.com");

    [Fact]
    public void Constructor_Should_CreateUser_WithValidState()
    {
        DateTime oldDateTime = DateTime.UtcNow;

        User user = CreateValidUser();

        user.CreatedAt.Should().BeAfter(oldDateTime);
        user.UpdatedAt.Should().BeAfter(oldDateTime);

        user.Id.Should().NotBe(Guid.Empty);

        user.UserName.Should().Be("john_doe");
        user.EmailHash.Should().Be("email_hash");
        user.PasswordHash.Should().Be("password_hash");

        user.IsLocked.Should().BeFalse();
        user.Role.Should().Be(SystemRole.User);
    }

    [Fact]
    public void SetUserName_Should_UpdateUserName_AndAudit()
    {
        User user = CreateValidUser();
        DateTime oldUpdatedAt = user.UpdatedAt;

        user.SetUserName("new_name");

        user.UserName.Should().Be("new_name");
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SetUserName_Should_Throw_WhenInvalid(string value)
    {
        User user = CreateValidUser();

        Action act = () => user.SetUserName(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void SetEmailHash_Should_UpdateEmailHash_AndAudit()
    {
        User user = CreateValidUser();
        DateTime oldUpdatedAt = user.UpdatedAt;

        user.SetEmailHash("new_email_hash");

        user.EmailHash.Should().Be("new_email_hash");
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SetEmailHash_Should_Throw_WhenInvalid(string value)
    {
        User user = CreateValidUser();

        Action act = () => user.SetEmailHash(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void SetPasswordHash_Should_UpdatePasswordHash_AndAudit()
    {
        User user = CreateValidUser();
        DateTime oldUpdatedAt = user.UpdatedAt;

        user.SetPasswordHash("new_password_hash");

        user.PasswordHash.Should().Be("new_password_hash");
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SetPasswordHash_Should_Throw_WhenInvalid(string value)
    {
        User user = CreateValidUser();

        Action act = () => user.SetPasswordHash(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void SetEncryptedData_Should_SetData_AndAudit()
    {
        User user = CreateValidUser();
        DateTime oldUpdatedAt = user.UpdatedAt;
        byte[] encrypted = [1, 2, 3];

        user.SetEncryptedData(encrypted);

        user.EncryptedData.Should().BeEquivalentTo(encrypted);
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Fact]
    public void SetEncryptedData_Should_Throw_WhenNull()
    {
        User user = CreateValidUser();

        Action act = () => user.SetEncryptedData(null!);

        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void SetEncryptedData_Should_Throw_WhenEmpty()
    {
        User user = CreateValidUser();

        Action act = () => user.SetEncryptedData([]);

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void SetSensitiveData_Should_StoreSensitiveData()
    {
        User user = CreateValidUser();
        var sensitive = CreateValidUserSensitive();

        user.SetSensitiveData(sensitive);

        user.SensitiveData.Should().Be(sensitive);
    }

    [Fact]
    public void ClearSensitiveData_Should_RemoveSensitiveData()
    {
        User user = CreateValidUser();
        UserSensitive sensitive = CreateValidUserSensitive();
        user.SetSensitiveData(sensitive);

        user.ClearSensitiveData();

        user.SensitiveData.Should().BeNull();
    }
}
