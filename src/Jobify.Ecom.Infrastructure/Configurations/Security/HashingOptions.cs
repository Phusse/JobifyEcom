namespace Jobify.Ecom.Infrastructure.Configurations.Security;

internal record HashingOptions
{
    public string EmailHmacKey { get; init; } = string.Empty;
    public int PasswordWorkFactor { get; init; } = 12;
}
