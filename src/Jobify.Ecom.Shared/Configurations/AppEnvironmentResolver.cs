using Jobify.Ecom.Shared.Enums;
using Microsoft.Extensions.Configuration;

namespace Jobify.Ecom.Shared.Configurations;

public static class AppEnvironmentResolver
{
    private const string Key = "APP_ENVIRONMENT";

    public static AppEnvironment Resolve(IConfiguration configuration)
    {
        string? value = configuration[Key];

        if (!Enum.TryParse<AppEnvironment>(value, ignoreCase: true, out AppEnvironment env))
            throw new InvalidOperationException($"Invalid {Key}. Allowed values: Local, Test, Production");

        return env;
    }
}
