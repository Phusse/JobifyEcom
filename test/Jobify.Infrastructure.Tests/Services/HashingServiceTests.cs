using FluentAssertions;
using Jobify.Infrastructure.Configurations.Security;
using Jobify.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Jobify.Infrastructure.Tests.Services;

public class HashingServiceTests
{
    private static HashingService CreateService(string emailHmacKey = "super-secret-hmac-key", int passwordWorkFactor = 10)
    {
        IOptions<HashingOptions> options = Options.Create(new HashingOptions
        {
            EmailHmacKey = emailHmacKey,
            PasswordWorkFactor = passwordWorkFactor,
        });

        return new HashingService(options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailHmacKeyIsMissing()
    {
        IOptions<HashingOptions> options = Options.Create(new HashingOptions
        {
            EmailHmacKey = "",
            PasswordWorkFactor = 10,
        });

        Action act = () => _ = new HashingService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*EmailHmacKey*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPasswordWorkFactorIsInvalid()
    {
        IOptions<HashingOptions> options = Options.Create(new HashingOptions
        {
            EmailHmacKey = "valid-key",
            PasswordWorkFactor = 0,
        });

        Action act = () => _ = new HashingService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*PasswordWorkFactor*");
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnDifferentHash_ForSamePassword()
    {
        HashingService service = CreateService();
        string password = "StrongPassword123!";

        string hash1 = await service.HashPasswordAsync(password);
        string hash2 = await service.HashPasswordAsync(password);

        hash1.Should().NotBeNullOrWhiteSpace();
        hash2.Should().NotBeNullOrWhiteSpace();
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnTrue_ForCorrectPassword()
    {
        HashingService service = CreateService();
        string password = "StrongPassword123!";
        string hash = await service.HashPasswordAsync(password);

        bool result = await service.VerifyPasswordAsync(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnFalse_ForWrongPassword()
    {
        HashingService service = CreateService();
        string password = "StrongPassword123!";
        string hash = await service.HashPasswordAsync(password);

        bool result = await service.VerifyPasswordAsync("WrongPassword!", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void HashEmail_ShouldBeDeterministic_ForSameEmail()
    {
        HashingService service = CreateService();
        string email = "user@example.com";

        string hash1 = service.HashEmail(email);
        string hash2 = service.HashEmail(email);

        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64);
    }

    [Fact]
    public void HashEmail_ShouldProduceDifferentHashes_ForDifferentEmails()
    {
        HashingService service = CreateService();

        string hash1 = service.HashEmail("user1@example.com");
        string hash2 = service.HashEmail("user2@example.com");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyEmail_ShouldReturnTrue_ForCorrectEmail()
    {
        HashingService service = CreateService();
        string email = "user@example.com";
        string hash = service.HashEmail(email);

        bool result = service.VerifyEmail(email, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_ShouldReturnFalse_ForIncorrectEmail()
    {
        HashingService service = CreateService();
        string hash = service.HashEmail("user@example.com");

        bool result = service.VerifyEmail("attacker@example.com", hash);

        result.Should().BeFalse();
    }
}
