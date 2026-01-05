using FluentAssertions;
using Jobify.Ecom.Infrastructure.Configurations.Security;
using Jobify.Ecom.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Jobify.Ecom.Infrastructure.Tests.Services;

public class HashingServiceTests
{
    private static HashingService CreateService(
        string emailHmacKey = "super-secret-hmac-key",
        int passwordWorkFactor = 10)
    {
        var options = Options.Create(new HashingOptions
        {
            EmailHmacKey = emailHmacKey,
            PasswordWorkFactor = passwordWorkFactor
        });

        return new HashingService(options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailHmacKeyIsMissing()
    {
        // Arrange
        var options = Options.Create(new HashingOptions
        {
            EmailHmacKey = "",
            PasswordWorkFactor = 10
        });

        // Act
        Action act = () => new HashingService(options);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*EmailHmacKey*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPasswordWorkFactorIsInvalid()
    {
        // Arrange
        var options = Options.Create(new HashingOptions
        {
            EmailHmacKey = "valid-key",
            PasswordWorkFactor = 0
        });

        // Act
        Action act = () => new HashingService(options);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*PasswordWorkFactor*");
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnDifferentHash_ForSamePassword()
    {
        // Arrange
        var service = CreateService();
        string password = "StrongPassword123!";

        // Act
        string hash1 = await service.HashPasswordAsync(password);
        string hash2 = await service.HashPasswordAsync(password);

        // Assert
        hash1.Should().NotBeNullOrWhiteSpace();
        hash2.Should().NotBeNullOrWhiteSpace();
        hash1.Should().NotBe(hash2); // BCrypt uses salt
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        var service = CreateService();
        string password = "StrongPassword123!";
        string hash = await service.HashPasswordAsync(password);

        // Act
        bool result = await service.VerifyPasswordAsync(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnFalse_ForWrongPassword()
    {
        // Arrange
        var service = CreateService();
        string password = "StrongPassword123!";
        string hash = await service.HashPasswordAsync(password);

        // Act
        bool result = await service.VerifyPasswordAsync("WrongPassword!", hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashEmail_ShouldBeDeterministic_ForSameEmail()
    {
        // Arrange
        var service = CreateService();
        string email = "user@example.com";

        // Act
        string hash1 = service.HashEmail(email);
        string hash2 = service.HashEmail(email);

        // Assert
        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64); // SHA-256 hex string
    }

    [Fact]
    public void HashEmail_ShouldProduceDifferentHashes_ForDifferentEmails()
    {
        // Arrange
        var service = CreateService();

        // Act
        string hash1 = service.HashEmail("user1@example.com");
        string hash2 = service.HashEmail("user2@example.com");

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyEmail_ShouldReturnTrue_ForCorrectEmail()
    {
        // Arrange
        var service = CreateService();
        string email = "user@example.com";
        string hash = service.HashEmail(email);

        // Act
        bool result = service.VerifyEmail(email, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_ShouldReturnFalse_ForIncorrectEmail()
    {
        // Arrange
        var service = CreateService();
        string hash = service.HashEmail("user@example.com");

        // Act
        bool result = service.VerifyEmail("attacker@example.com", hash);

        // Assert
        result.Should().BeFalse();
    }
}
