using Microsoft.AspNetCore.Mvc;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Enums;
using JobifyEcom.Services;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    #region GetCurrentUser Tests

    [Test]
    public async Task GetCurrentUser_ReturnsOkWithUserProfile()
    {
        // Arrange
        var userProfile = new UserProfileResponse 
        { 
            Id = Guid.NewGuid(), 
            Name = "John Doe", 
            Email = "john@test.com",
            Bio = "A test user",
            Roles = [SystemRole.User],
            CreatedAt = DateTime.UtcNow
        };
        var serviceResult = new ServiceResult<UserProfileResponse>
        {
            Data = userProfile,
            Message = "Profile retrieved"
        };

        _userServiceMock.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.GetCurrentUserAsync(), Times.Once);
    }

    #endregion

    #region GetUserById Tests

    [Test]
    public async Task GetUserById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = new { Id = userId, Name = "Test User" },
            Message = "User found"
        };

        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
    }

    #endregion

    #region UpdateUser Tests

    [Test]
    public async Task UpdateUser_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new UserProfileUpdateRequest { Name = "Updated Name" };
        var updatedProfile = new UserProfileResponse 
        { 
            Id = Guid.NewGuid(), 
            Name = "Updated Name", 
            Email = "test@test.com",
            Bio = "Updated bio",
            Roles = [SystemRole.User],
            CreatedAt = DateTime.UtcNow
        };
        var serviceResult = new ServiceResult<UserProfileResponse>
        {
            Data = updatedProfile,
            Message = "Profile updated"
        };

        _userServiceMock.Setup(s => s.UpdateCurrentUserAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UpdateUser(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.UpdateCurrentUserAsync(request), Times.Once);
    }

    #endregion

    #region ConfirmEmail Tests

    [Test]
    public async Task ConfirmEmailLink_WithValidToken_ReturnsOkResult()
    {
        // Arrange
        var request = new EmailConfirmRequest { Email = "test@test.com", Token = Guid.NewGuid() };
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Email confirmed"
        };

        _userServiceMock.Setup(s => s.ConfirmEmailAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ConfirmEmailLink(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ConfirmEmail_WithValidToken_ReturnsOkResult()
    {
        // Arrange
        var request = new EmailConfirmRequest { Email = "test@test.com", Token = Guid.NewGuid() };
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Email confirmed"
        };

        _userServiceMock.Setup(s => s.ConfirmEmailAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ConfirmEmail(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.ConfirmEmailAsync(request), Times.Once);
    }

    #endregion

    #region LockUser Tests

    [Test]
    public async Task LockUser_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "User locked"
        };

        _userServiceMock.Setup(s => s.LockUserAsync(userId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.LockUser(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.LockUserAsync(userId), Times.Once);
    }

    #endregion

    #region UnlockUser Tests

    [Test]
    public async Task UnlockUser_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "User unlocked"
        };

        _userServiceMock.Setup(s => s.UnlockUserAsync(userId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UnlockUser(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.UnlockUserAsync(userId), Times.Once);
    }

    #endregion

    #region RequestPasswordReset Tests

    [Test]
    public async Task RequestPasswordReset_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = new { Token = Guid.NewGuid(), ExpiresAt = DateTime.UtcNow.AddHours(1) },
            Message = "Reset token generated"
        };

        _userServiceMock.Setup(s => s.RequestPasswordResetAsync(userId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.RequestPasswordReset(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.RequestPasswordResetAsync(userId), Times.Once);
    }

    #endregion

    #region ResetPassword Tests

    [Test]
    public async Task ResetPassword_WithValidTokenAndPassword_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new PasswordResetRequest { Token = Guid.NewGuid(), NewPassword = "newPass123!" };
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Password reset successful"
        };

        _userServiceMock.Setup(s => s.ResetPasswordAsync(userId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ResetPassword(userId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.ResetPasswordAsync(userId, request), Times.Once);
    }

    #endregion

    #region DeleteCurrentUser Tests

    [Test]
    public async Task DeleteCurrentUser_ReturnsOkResult()
    {
        // Arrange
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Account deleted"
        };

        _userServiceMock.Setup(s => s.DeleteCurrentUserAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteCurrentUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.DeleteCurrentUserAsync(), Times.Once);
    }

    #endregion

    #region DeleteUser Tests

    [Test]
    public async Task DeleteUser_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "User deleted"
        };

        _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _userServiceMock.Verify(s => s.DeleteUserAsync(userId), Times.Once);
    }

    #endregion
}
