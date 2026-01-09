namespace Jobify.Infrastructure.Configurations.Security;

internal record DataEncryptionOptions
{
    public string Key { get; init; } = string.Empty;
}
