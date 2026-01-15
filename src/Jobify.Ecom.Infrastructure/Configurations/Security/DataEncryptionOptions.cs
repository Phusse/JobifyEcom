namespace Jobify.Ecom.Infrastructure.Configurations.Security;

internal record DataEncryptionOptions
{
    public byte CurrentKeyVersion { get; set; } = 1;
    public Dictionary<byte, string> Keys { get; set; } = [];
}
