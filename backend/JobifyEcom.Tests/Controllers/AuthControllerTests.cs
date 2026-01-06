using Microsoft.AspNetCore.Mvc;
using Moq;
using JobifyEcom.Controllers;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Auth;
using JobifyEcom.Services;
using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    #region Login Tests

    [Test]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Password = "password123" };
        var tokenResponse = new TokenResponse
        {
            AccessToken = "access-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddHours(1),
            RefreshToken = "refresh-token",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var serviceResult = new ServiceResult<TokenResponse>
        {
            Data = tokenResponse,
            Message = "Login successful",
            MessageId = "AUTH_001"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
    }

    [Test]
    public async Task Login_CallsAuthService_WithCorrectRequest()
    {
        // Arrange
        var request = new LoginRequest { Email = "user@test.com", Password = "securePass!" };
        var serviceResult = new ServiceResult<TokenResponse>
        {
            Data = null,
            Message = "Invalid credentials"
        };

        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(serviceResult);

        // Act
        await _controller.Login(request);

        // Assert
        _authServiceMock.Verify(s => s.LoginAsync(It.Is<LoginRequest>(r => 
            r.Email == "user@test.com" && r.Password == "securePass!")), Times.Once);
    }

    #endregion

    #region RefreshToken Tests

    [Test]
    public async Task RefreshToken_WithValidToken_ReturnsOkWithNewTokens()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
        var tokenResponse = new TokenResponse
        {
            AccessToken = "new-access-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddHours(1),
            RefreshToken = "new-refresh-token",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var serviceResult = new ServiceResult<TokenResponse>
        {
            Data = tokenResponse,
            Message = "Token refreshed"
        };

        _authServiceMock.Setup(s => s.RefreshTokenAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _authServiceMock.Verify(s => s.RefreshTokenAsync(request), Times.Once);
    }

    [Test]
    public async Task RefreshToken_CallsAuthService_WithCorrectRequest()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = "test-token" };
        var serviceResult = new ServiceResult<TokenResponse> { Data = null };

        _authServiceMock.Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>())).ReturnsAsync(serviceResult);

        // Act
        await _controller.RefreshToken(request);

        // Assert
        _authServiceMock.Verify(s => s.RefreshTokenAsync(It.Is<RefreshTokenRequest>(r => 
            r.RefreshToken == "test-token")), Times.Once);
    }

    #endregion

    #region Register Tests

    [Test]
    public async Task Register_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Name = "John Doe",
            Email = "john@example.com", 
            Password = "password123" 
        };
        var registerResponse = new RegisterResponse { ConfirmationLink = "https://api.test/confirm?token=abc123" };
        var serviceResult = new ServiceResult<RegisterResponse>
        {
            Data = registerResponse,
            Message = "Registration successful"
        };

        _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        var createdResult = result as CreatedResult;
        Assert.That(createdResult?.Location, Is.EqualTo("https://api.test/confirm?token=abc123"));
    }

    [Test]
    public async Task Register_StripsDataFromResponse_OnlyKeepsLocationHeader()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Name = "Jane Doe",
            Email = "jane@example.com", 
            Password = "password456" 
        };
        var registerResponse = new RegisterResponse { ConfirmationLink = "https://api.test/confirm?token=xyz789" };
        var serviceResult = new ServiceResult<RegisterResponse>
        {
            Data = registerResponse,
            Message = "User registered"
        };

        _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        // The Data should be stripped (set to null) before response
        Assert.That(serviceResult.Data, Is.Null);
        _authServiceMock.Verify(s => s.RegisterAsync(request), Times.Once);
    }

    #endregion

    #region Logout Tests

    [Test]
    public async Task Logout_ReturnsOkResult()
    {
        // Arrange
        var serviceResult = new ServiceResult<object>
        {
            Data = null,
            Message = "Logout successful"
        };

        _authServiceMock.Setup(s => s.LogoutAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Logout();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _authServiceMock.Verify(s => s.LogoutAsync(), Times.Once);
    }

    [Test]
    public async Task Logout_CallsAuthServiceLogout_ExactlyOnce()
    {
        // Arrange
        var serviceResult = new ServiceResult<object> { Message = "Logged out" };
        _authServiceMock.Setup(s => s.LogoutAsync()).ReturnsAsync(serviceResult);

        // Act
        await _controller.Logout();

        // Assert
        _authServiceMock.Verify(s => s.LogoutAsync(), Times.Once);
    }

    #endregion
}
