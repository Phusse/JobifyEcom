using FluentAssertions;
using Jobify.Application.Services;
using Jobify.Infrastructure.Configurations.Security;
using Jobify.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Jobify.Infrastructure.Tests.Services;

public class HashingServiceTests
{
    private const string ValidHmacKey = "test-hmac-key-for-email-hashing-1234567890";
    private const int ValidWorkFactor = 4; // Low for fast tests

    private static HashingService CreateHashingService(
        string? emailHmacKey = ValidHmacKey,
        int passwordWorkFactor = ValidWorkFactor)
    {
        var options = new HashingOptions
        {
            EmailHmacKey = emailHmacKey ?? string.Empty,
            PasswordWorkFactor = passwordWorkFactor
        };

        return new HashingService(Options.Create(options));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithMissingHmacKey_ThrowsInvalidOperationException(string? hmacKey)
    {
        // Act
        var act = () => CreateHashingService(emailHmacKey: hmacKey);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*EmailHmacKey must be provided*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithZeroOrNegativeWorkFactor_ThrowsInvalidOperationException(int workFactor)
    {
        // Act
        var act = () => CreateHashingService(passwordWorkFactor: workFactor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*PasswordWorkFactor must be greater than 0*");
    }

    [Fact]
    public async Task HashPasswordAsync_ReturnsHash()
    {
        // Arrange
        var service = CreateHashingService();
        var password = "MySecurePassword123!";

        // Act
        var hash = await service.HashPasswordAsync(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$2"); // BCrypt hash prefix
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var service = CreateHashingService();
        var password = "MySecurePassword123!";
        var hash = await service.HashPasswordAsync(password);

        // Act
        var result = await service.VerifyPasswordAsync(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var service = CreateHashingService();
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = await service.HashPasswordAsync(password);

        // Act
        var result = await service.VerifyPasswordAsync(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashEmail_ReturnsConsistentHash()
    {
        // Arrange
        var service = CreateHashingService();
        var email = "test@example.com";

        // Act
        var hash1 = service.HashEmail(email);
        var hash2 = service.HashEmail(email);

        // Assert
        hash1.Should().NotBeNullOrEmpty();
        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64); // SHA256 produces 64 hex chars
    }

    [Fact]
    public void HashEmail_DifferentEmails_ProduceDifferentHashes()
    {
        // Arrange
        var service = CreateHashingService();
        var email1 = "test1@example.com";
        var email2 = "test2@example.com";

        // Act
        var hash1 = service.HashEmail(email1);
        var hash2 = service.HashEmail(email2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyEmail_WithMatchingEmail_ReturnsTrue()
    {
        // Arrange
        var service = CreateHashingService();
        var email = "test@example.com";
        var hash = service.HashEmail(email);

        // Act
        var result = service.VerifyEmail(email, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_WithDifferentEmail_ReturnsFalse()
    {
        // Arrange
        var service = CreateHashingService();
        var email = "test@example.com";
        var differentEmail = "other@example.com";
        var hash = service.HashEmail(email);

        // Act
        var result = service.VerifyEmail(differentEmail, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashEmail_IsCaseInsensitiveToHmacKey()
    {
        // Arrange - two services with same key should produce same hash
        var service1 = CreateHashingService();
        var service2 = CreateHashingService();
        var email = "test@example.com";

        // Act
        var hash1 = service1.HashEmail(email);
        var hash2 = service2.HashEmail(email);

        // Assert
        hash1.Should().Be(hash2);
    }
}
