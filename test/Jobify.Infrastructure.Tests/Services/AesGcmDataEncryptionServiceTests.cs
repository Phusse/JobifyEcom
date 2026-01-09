using System.Security.Cryptography;
using FluentAssertions;
using Jobify.Infrastructure.Configurations.Security;
using Jobify.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Jobify.Infrastructure.Tests.Services;

public class AesGcmDataEncryptionServiceTests
{
    private static byte[] GenerateValidKey()
    {
        byte[] key = new byte[32];
        Random.Shared.NextBytes(key);
        return key;
    }

    private static AesGcmDataEncryptionService CreateService(byte[]? key = null)
    {
        key ??= GenerateValidKey();

        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            Key = Convert.ToBase64String(key),
        });

        return new AesGcmDataEncryptionService(options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsMissing()
    {
        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            Key = string.Empty,
        });

        Action act = () => _ = new AesGcmDataEncryptionService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*DataEncryption:Key*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsNotBase64()
    {
        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            Key = "not-base64!!!",
        });

        Action act = () => _ = new AesGcmDataEncryptionService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*Base64*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsWrongLength()
    {
        byte[] shortKey = new byte[16];
        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            Key = Convert.ToBase64String(shortKey),
        });

        Action act = () => _ = new AesGcmDataEncryptionService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*32 bytes*");
    }

    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalPlaintext()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = [.. "Sensitive data payload".Select(c => (byte)c)];

        byte[] encrypted = service.EncryptData(plaintext);
        byte[] decrypted = service.DecryptData(encrypted);

        decrypted.Should().Equal(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldProduceDifferentCiphertext_ForSamePlaintext()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = [.. "Same input data".Select(c => (byte)c)];

        byte[] encrypted1 = service.EncryptData(plaintext);
        byte[] encrypted2 = service.EncryptData(plaintext);

        encrypted1.Should().NotEqual(encrypted2);
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenCiphertextIsTampered()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = [.. "Top secret".Select(c => (byte)c)];
        byte[] encrypted = service.EncryptData(plaintext);

        encrypted[^1] ^= 0xFF;

        Action act = () => service.DecryptData(encrypted);

        act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void EncryptData_ShouldThrow_WhenKeyIsInvalidLength()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = [1, 2, 3];
        byte[] invalidKey = new byte[16];

        Action act = () => service.EncryptData(plaintext, invalidKey);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*32 bytes*");
    }

    [Fact]
    public void DecryptData_ShouldThrow_WhenKeyIsInvalidLength()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = [1, 2, 3];
        byte[] encrypted = service.EncryptData(plaintext);
        byte[] invalidKey = new byte[16];

        Action act = () => service.DecryptData(encrypted, invalidKey);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*32 bytes*");
    }

    [Fact]
    public void DecryptData_ShouldThrow_WhenEncryptedDataIsTooShort()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] invalidData = new byte[10];

        Action act = () => service.DecryptData(invalidData);

        act.ShouldThrow<ArgumentException>().WithMessage("*Invalid encrypted data*");
    }
}
