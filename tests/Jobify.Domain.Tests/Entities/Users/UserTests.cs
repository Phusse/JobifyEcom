using FluentAssertions;
using Jobify.Domain.Entities.Users;
using Xunit;

namespace Jobify.Domain.Tests.Entities.Users;

public class UserTests
{
    private const string ValidUserName = "testuser";
    private const string ValidEmailHash = "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"; // 64 chars
    private const string ValidPasswordHash = "hashedpassword123";

    [Fact]
    public void Constructor_WithValidData_CreatesUser()
    {
        // Arrange & Act
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Assert
        user.UserName.Should().Be(ValidUserName);
        user.EmailHash.Should().Be(ValidEmailHash);
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetUserName_WithNullOrEmpty_ThrowsArgumentException(string? userName)
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Act
        var act = () => user.SetUserName(userName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void SetUserName_WhenTooShort_ThrowsArgumentException()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var shortName = new string('a', UserLimits.UserNameMinLength - 1);

        // Act
        var act = () => user.SetUserName(shortName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*between {UserLimits.UserNameMinLength} and {UserLimits.UserNameMaxLength}*");
    }

    [Fact]
    public void SetUserName_WhenTooLong_ThrowsArgumentException()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var longName = new string('a', UserLimits.UserNameMaxLength + 1);

        // Act
        var act = () => user.SetUserName(longName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*between {UserLimits.UserNameMinLength} and {UserLimits.UserNameMaxLength}*");
    }

    [Fact]
    public void SetUserName_WithValidName_UpdatesUserNameAndAudit()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;
        var newName = "newusername"; 

        // Allow time to pass
        Thread.Sleep(10);

        // Act
        user.SetUserName(newName);

        // Assert
        user.UserName.Should().Be(newName);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetEmailHash_WithNullOrEmpty_ThrowsArgumentException(string? emailHash)
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Act
        var act = () => user.SetEmailHash(emailHash!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Theory]
    [InlineData(63)]  // Too short
    [InlineData(65)]  // Too long
    public void SetEmailHash_WithInvalidLength_ThrowsArgumentException(int length)
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var invalidHash = new string('a', length);

        // Act
        var act = () => user.SetEmailHash(invalidHash);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*exactly {UserLimits.EmailHashMaxLength} characters*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetPasswordHash_WithNullOrEmpty_ThrowsArgumentException(string? passwordHash)
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Act
        var act = () => user.SetPasswordHash(passwordHash!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void SetPasswordHash_WhenTooLong_ThrowsArgumentException()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var longHash = new string('a', UserLimits.PasswordHashMaxLength + 1);

        // Act
        var act = () => user.SetPasswordHash(longHash);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*cannot exceed {UserLimits.PasswordHashMaxLength}*");
    }

    [Fact]
    public void SetEncryptedData_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Act
        var act = () => user.SetEncryptedData(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetEncryptedData_WithEmptyArray_ThrowsArgumentException()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);

        // Act
        var act = () => user.SetEncryptedData([]);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void SetEncryptedData_WithValidData_UpdatesEncryptedDataAndAudit()
    {
        // Arrange
        var user = new User(ValidUserName, ValidEmailHash, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;
        var encryptedData = new byte[] { 1, 2, 3, 4 };

        Thread.Sleep(10);

        // Act
        user.SetEncryptedData(encryptedData);

        // Assert
        user.EncryptedData.Should().BeEquivalentTo(encryptedData);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
