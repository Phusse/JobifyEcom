using FluentAssertions;
using Jobify.Ecom.Domain.Entities.Users;

namespace Jobify.Ecom.Domain.Tests.Entities.Users;

public class UserSensitiveTests
{
    [Fact]
    public void Create_Should_Return_UserSensitive_With_Normalized_Values()
    {
        UserSensitive sensitive = UserSensitive.Create(
            firstName: " John ",
            middleName: "  Smith ",
            lastName: " Doe ",
            email: "John.Doe@Example.com"
        );

        sensitive.FirstName.Should().Be("John");
        sensitive.MiddleName.Should().Be("Smith");
        sensitive.LastName.Should().Be("Doe");
        sensitive.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void Create_Should_Allow_Null_MiddleName()
    {
        UserSensitive sensitive = UserSensitive.Create(
            firstName: "John",
            middleName: null,
            lastName: "Doe",
            email: "john@doe.com"
        );

        sensitive.MiddleName.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_FirstName_Is_Invalid(string value)
    {
        Action act = () => UserSensitive.Create(value, null, "Doe", "john@doe.com");

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "firstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_LastName_Is_Invalid(string value)
    {
        Action act = () => UserSensitive.Create("John", null, value, "john@doe.com");

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "lastName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_Email_Is_Missing(string value)
    {
        Action act = () => UserSensitive.Create("John", null, "Doe", value);

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "email");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("john@")]
    [InlineData("@doe.com")]
    [InlineData("john@@doe.com")]
    public void Create_Should_Throw_When_Email_Format_Is_Invalid(string email)
    {
        Action act = () => UserSensitive.Create("John", null, "Doe", email);

        act.ShouldThrow<ArgumentException>().Where(e => e.ParamName == "email");
    }
}
