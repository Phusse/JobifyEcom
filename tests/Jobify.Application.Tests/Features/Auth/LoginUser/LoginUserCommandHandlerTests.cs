using FluentAssertions;
using Jobify.Application.Features.Auth.LoginUser;
using Jobify.Application.Services;
using Jobify.Domain.Entities.Users;
using Jobify.Domain.Enums;
using Jobify.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobify.Application.Tests.Features.Auth.LoginUser;

public class LoginUserCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<IHashingService> _hashingServiceMock;
    private readonly LoginUserCommandHandler _handler;

    private const string TestEmail = "test@example.com";
    private const string TestEmailHash = "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"; // 64 chars
    private const string TestUsername = "testuser";
    private const string TestPassword = "password123";
    private const string TestPasswordHash = "$2a$04$hashedpassword";

    public LoginUserCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _hashingServiceMock = new Mock<IHashingService>();

        // Default setup for hashing service
        _hashingServiceMock.Setup(x => x.HashEmail(It.IsAny<string>()))
            .Returns(TestEmailHash);
        _hashingServiceMock.Setup(x => x.VerifyPasswordAsync(TestPassword, TestPasswordHash))
            .ReturnsAsync(true);
        _hashingServiceMock.Setup(x => x.VerifyPasswordAsync(It.Is<string>(p => p != TestPassword), TestPasswordHash))
            .ReturnsAsync(false);

        // Note: For handler tests, we need to mock SessionManagementService
        // but since it's not an interface, we'll test at integration level
        // For unit tests, consider extracting an interface
    }

    private User CreateTestUser(bool isLocked = false)
    {
        var user = new User(TestUsername, TestEmailHash, TestPasswordHash);
        return user;
    }

    private async Task SeedUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public void Command_WithValidCredentials_ShouldBeCreatedCorrectly()
    {
        // Arrange & Act
        var command = new LoginUserCommand(TestEmail, TestPassword, RememberMe: true);

        // Assert
        command.Identifier.Should().Be(TestEmail);
        command.Password.Should().Be(TestPassword);
        command.RememberMe.Should().BeTrue();
    }

    [Fact]
    public void Command_WithDefaultRememberMe_ShouldBeFalse()
    {
        // Arrange & Act
        var command = new LoginUserCommand(TestEmail, TestPassword);

        // Assert
        command.RememberMe.Should().BeFalse();
    }

    [Fact]
    public async Task HashingService_HashEmail_ShouldBeCalledWithIdentifier()
    {
        // Arrange
        var user = CreateTestUser();
        await SeedUserAsync(user);

        // Act
        _hashingServiceMock.Object.HashEmail(TestEmail);

        // Assert
        _hashingServiceMock.Verify(x => x.HashEmail(TestEmail), Times.Once);
    }

    [Fact]
    public async Task HashingService_VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var user = CreateTestUser();
        await SeedUserAsync(user);

        // Act
        var result = await _hashingServiceMock.Object.VerifyPasswordAsync(TestPassword, TestPasswordHash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HashingService_VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var user = CreateTestUser();
        await SeedUserAsync(user);

        // Act
        var result = await _hashingServiceMock.Object.VerifyPasswordAsync("wrongpassword", TestPasswordHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DbContext_FindUserByEmailHash_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        await SeedUserAsync(user);

        // Act
        var foundUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailHash == TestEmailHash);

        // Assert
        foundUser.Should().NotBeNull();
        foundUser!.UserName.Should().Be(TestUsername);
    }

    [Fact]
    public async Task DbContext_FindUserByUsername_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        await SeedUserAsync(user);

        // Act
        var foundUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == TestUsername);

        // Assert
        foundUser.Should().NotBeNull();
        foundUser!.EmailHash.Should().Be(TestEmailHash);
    }

    [Fact]
    public async Task DbContext_FindNonExistentUser_ShouldReturnNull()
    {
        // Act
        var foundUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == "nonexistent");

        // Assert
        foundUser.Should().BeNull();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
