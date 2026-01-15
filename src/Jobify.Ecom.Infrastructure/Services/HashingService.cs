using System.Security.Cryptography;
using System.Text;
using BCryption = BCrypt.Net.BCrypt;
using Jobify.Ecom.Application.Services;
using Jobify.Ecom.Infrastructure.Configurations.Security;
using Microsoft.Extensions.Options;

namespace Jobify.Ecom.Infrastructure.Services;

internal class HashingService : IHashingService
{
    private readonly byte[] _emailHmacKey;
    private readonly int _passwordWorkFactor;

    public HashingService(IOptions<HashingOptions> options)
    {
        HashingOptions opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.EmailHmacKey))
            throw new InvalidOperationException("Hashing:EmailHmacKey must be provided in configuration.");

        if (opts.PasswordWorkFactor <= 0)
            throw new InvalidOperationException("Hashing:PasswordWorkFactor must be greater than 0.");

        _emailHmacKey = Encoding.UTF8.GetBytes(opts.EmailHmacKey);
        _passwordWorkFactor = opts.PasswordWorkFactor;
    }

    public async Task<string> HashPasswordAsync(string password)
        => await Task.Run(() => BCryption.HashPassword(password, _passwordWorkFactor));

    public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        => await Task.Run(() => BCryption.Verify(password, hashedPassword));

    public string HashEmail(string email)
    {
        using HMACSHA256 hmac = new(_emailHmacKey);
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(email));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool VerifyEmail(string email, string hashedEmail)
    {
        string computed = HashEmail(email);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(hashedEmail)
        );
    }
}
