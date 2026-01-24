using FluentAssertions;
using FluentValidation.TestHelper;
using Jobify.Application.Features.Auth.LoginUser;
using Jobify.Domain.Entities.Users;
using Xunit;

namespace Jobify.Application.Tests.Features.Auth.LoginUser;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithNullOrEmptyIdentifier_ShouldHaveError(string? identifier)
    {
        // Arrange
        var command = new LoginUserCommand(identifier!, "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Identifier);
    }

    [Fact]
    public void Validate_WithIdentifierExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longIdentifier = new string('a', UserLimits.IdentifierIdMaxLength + 1);
        var command = new LoginUserCommand(longIdentifier, "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Identifier);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithNullOrEmptyPassword_ShouldHaveError(string? password)
    {
        // Arrange
        var command = new LoginUserCommand("testuser", password!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WithValidEmailAsIdentifier_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginUserCommand("test@example.com", "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithRememberMeTrue_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "password123", RememberMe: true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
