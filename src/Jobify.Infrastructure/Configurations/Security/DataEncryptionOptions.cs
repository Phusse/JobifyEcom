namespace Jobify.Infrastructure.Configurations.Security;

internal record DataEncryptionOptions
{
    public byte CurrentKeyVersion { get; init; } = 1;
    public Dictionary<byte, string> Keys { get; init; } = [];
}
