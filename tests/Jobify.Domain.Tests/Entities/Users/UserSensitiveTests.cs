using FluentAssertions;
using Jobify.Domain.Entities.Users;
using Xunit;

namespace Jobify.Domain.Tests.Entities.Users;

public class UserSensitiveTests
{
    [Fact]
    public void Create_WithValidData_CreatesUserSensitive()
    {
        // Arrange & Act
        var sensitive = UserSensitive.Create("John", "Middle", "Doe", "john.doe@example.com");

        // Assert
        sensitive.FirstName.Should().Be("John");
        sensitive.MiddleName.Should().Be("Middle");
        sensitive.LastName.Should().Be("Doe");
        sensitive.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void Create_WithNullMiddleName_Succeeds()
    {
        // Arrange & Act
        var sensitive = UserSensitive.Create("John", null, "Doe", "john.doe@example.com");

        // Assert
        sensitive.MiddleName.Should().BeNull();
    }

    [Fact]
    public void Create_LowercasesEmail()
    {
        // Arrange & Act - Note: validation happens before trimming
        var sensitive = UserSensitive.Create("John", null, "Doe", "JOHN.DOE@EXAMPLE.COM");

        // Assert - email is lowercased
        sensitive.Email.Should().Be("john.doe@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyFirstName_ThrowsArgumentException(string? firstName)
    {
        // Act
        var act = () => UserSensitive.Create(firstName!, null, "Doe", "test@example.com");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*First name is required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyLastName_ThrowsArgumentException(string? lastName)
    {
        // Act
        var act = () => UserSensitive.Create("John", null, lastName!, "test@example.com");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Last name is required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyEmail_ThrowsArgumentException(string? email)
    {
        // Act
        var act = () => UserSensitive.Create("John", null, "Doe", email!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email is required*");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@")]
    public void Create_WithInvalidEmailFormat_ThrowsArgumentException(string email)
    {
        // Act
        var act = () => UserSensitive.Create("John", null, "Doe", email);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email format is invalid*");
    }
}
