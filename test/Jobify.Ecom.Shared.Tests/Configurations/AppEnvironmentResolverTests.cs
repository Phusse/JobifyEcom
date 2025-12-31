using Jobify.Ecom.Shared.Configurations;
using Jobify.Ecom.Shared.Enums;
using Microsoft.Extensions.Configuration;

namespace Jobify.Ecom.Shared.Tests.Configurations;

public class AppEnvironmentResolverTests
{
    private static IConfiguration BuildConfig(string? value)
    {
        Dictionary<string, string?> data = [];

        if (value is not null)
            data["APP_ENVIRONMENT"] = value;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }

    [Theory]
    [InlineData("Local", AppEnvironment.Local)]
    [InlineData("local", AppEnvironment.Local)]
    [InlineData("Test", AppEnvironment.Test)]
    [InlineData("Production", AppEnvironment.Production)]
    public void Resolve_ReturnsExpectedEnvironment(string value, AppEnvironment expected)
    {
        IConfiguration config = BuildConfig(value);

        AppEnvironment result = AppEnvironmentResolver.Resolve(config);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Resolve_Throws_WhenEnvironmentIsInvalid()
    {
        IConfiguration config = BuildConfig("Invalid");

        Assert.Throws<InvalidOperationException>(() => AppEnvironmentResolver.Resolve(config));
    }

    [Fact]
    public void Resolve_Throws_WhenEnvironmentIsMissing()
    {
        IConfiguration config = BuildConfig(null);

        Assert.Throws<InvalidOperationException>(() => AppEnvironmentResolver.Resolve(config));
    }
}
