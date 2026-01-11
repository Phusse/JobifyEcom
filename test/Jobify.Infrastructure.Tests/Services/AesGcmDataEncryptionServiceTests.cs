using System.Security.Cryptography;
using FluentAssertions;
using Jobify.Application.Enums;
using Jobify.Infrastructure.Configurations.Security;
using Jobify.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Jobify.Infrastructure.Tests.Services;

public class AesGcmDataEncryptionServiceTests
{
    private static byte[] GenerateValidKey()
    {
        byte[] key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return key;
    }

    private static AesGcmDataEncryptionService CreateService(Dictionary<byte, string>? keys = null, byte? currentVersion = null)
    {
        currentVersion ??= 1;

        keys ??= new Dictionary<byte, string>
        {
            [currentVersion.Value] = Convert.ToBase64String(GenerateValidKey())
        };

        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            CurrentKeyVersion = currentVersion.Value,
            Keys = keys
        });

        return new AesGcmDataEncryptionService(options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeysAreMissing()
    {
        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            Keys = []
        });

        Action act = () => _ = new AesGcmDataEncryptionService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*At least one key*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsWrongLength()
    {
        byte[] shortKey = new byte[16];
        IOptions<DataEncryptionOptions> options = Options.Create(new DataEncryptionOptions
        {
            CurrentKeyVersion = 1,
            Keys = new Dictionary<byte, string> { [1] = Convert.ToBase64String(shortKey) }
        });

        Action act = () => _ = new AesGcmDataEncryptionService(options);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*32 bytes*");
    }

    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalPlaintext()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = "Sensitive data payload"u8.ToArray();
        CryptoPurpose purpose = CryptoPurpose.UserSensitiveData;

        byte[] encrypted = service.Encrypt(plaintext, purpose);
        byte[] decrypted = service.Decrypt(encrypted, purpose);

        decrypted.Should().Equal(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldProduceDifferentCiphertext_ForSamePlaintext()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = "Same input data"u8.ToArray();
        CryptoPurpose purpose = CryptoPurpose.UserSensitiveData;

        byte[] encrypted1 = service.Encrypt(plaintext, purpose);
        byte[] encrypted2 = service.Encrypt(plaintext, purpose);

        encrypted1.Should().NotEqual(encrypted2);
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenCiphertextIsTampered()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = "Top secret"u8.ToArray();
        CryptoPurpose purpose = CryptoPurpose.UserSensitiveData;
        byte[] encrypted = service.Encrypt(plaintext, purpose);

        encrypted[^1] ^= 0xFF;

        Action act = () => service.Decrypt(encrypted, purpose);

        act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenPurposeIsDifferent()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] plaintext = "Sensitive data"u8.ToArray();

        byte[] encrypted = service.Encrypt(plaintext, (CryptoPurpose)1);

        Action act = () => service.Decrypt(encrypted, (CryptoPurpose)2);

        act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void Decrypt_ShouldSupportHistoricalKeys()
    {
        byte[] oldKey = GenerateValidKey();
        byte[] newKey = GenerateValidKey();

        var keys = new Dictionary<byte, string>
        {
            [1] = Convert.ToBase64String(oldKey),
            [2] = Convert.ToBase64String(newKey)
        };

        AesGcmDataEncryptionService oldService = CreateService(keys, 1);
        byte[] plaintext = "Old secret"u8.ToArray();
        byte[] encrypted = oldService.Encrypt(plaintext, CryptoPurpose.UserSensitiveData);

        AesGcmDataEncryptionService newService = CreateService(keys, 2);
        byte[] decrypted = newService.Decrypt(encrypted, CryptoPurpose.UserSensitiveData);

        decrypted.Should().Equal(plaintext);
        encrypted[0].Should().Be(1);
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenVersionIsUnknown()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] encrypted = [99, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];

        Action act = () => service.Decrypt(encrypted, CryptoPurpose.UserSensitiveData);

        act.ShouldThrow<InvalidOperationException>().WithMessage("*No key configured for version 99*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenDataIsTooShort()
    {
        AesGcmDataEncryptionService service = CreateService();
        byte[] invalidData = [1, 2, 3];

        Action act = () => service.Decrypt(invalidData, CryptoPurpose.UserSensitiveData);

        act.ShouldThrow<ArgumentException>().WithMessage("*Invalid encrypted data*");
    }
}
