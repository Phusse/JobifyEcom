using System.Security.Cryptography;
using FluentAssertions;
using Jobify.Ecom.Infrastructure.Configurations.Security;
using Jobify.Ecom.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Jobify.Ecom.Infrastructure.Tests.Services;

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

        var options = Options.Create(new DataEncryptionOptions
        {
            Key = Convert.ToBase64String(key)
        });

        return new AesGcmDataEncryptionService(options);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsMissing()
    {
        // Arrange
        var options = Options.Create(new DataEncryptionOptions
        {
            Key = ""
        });

        // Act
        Action act = () => new AesGcmDataEncryptionService(options);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*DataEncryption:Key*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsNotBase64()
    {
        // Arrange
        var options = Options.Create(new DataEncryptionOptions
        {
            Key = "not-base64!!!"
        });

        // Act
        Action act = () => new AesGcmDataEncryptionService(options);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*Base64*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsWrongLength()
    {
        // Arrange
        byte[] shortKey = new byte[16]; // AES-128
        var options = Options.Create(new DataEncryptionOptions
        {
            Key = Convert.ToBase64String(shortKey)
        });

        // Act
        Action act = () => new AesGcmDataEncryptionService(options);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*32 bytes*");
    }

    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalPlaintext()
    {
        // Arrange
        var service = CreateService();
        byte[] plaintext = "Sensitive data payload".Select(c => (byte)c).ToArray();

        // Act
        byte[] encrypted = service.EncryptData(plaintext);
        byte[] decrypted = service.DecryptData(encrypted);

        // Assert
        decrypted.Should().Equal(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldProduceDifferentCiphertext_ForSamePlaintext()
    {
        // Arrange
        var service = CreateService();
        byte[] plaintext = "Same input data".Select(c => (byte)c).ToArray();

        // Act
        byte[] encrypted1 = service.EncryptData(plaintext);
        byte[] encrypted2 = service.EncryptData(plaintext);

        // Assert
        encrypted1.Should().NotEqual(encrypted2); // nonce randomness
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenCiphertextIsTampered()
    {
        // Arrange
        var service = CreateService();
        byte[] plaintext = "Top secret".Select(c => (byte)c).ToArray();
        byte[] encrypted = service.EncryptData(plaintext);

        // Tamper with ciphertext
        encrypted[^1] ^= 0xFF;

        // Act
        Action act = () => service.DecryptData(encrypted);

        // Assert
        act.ShouldThrow<CryptographicException>();
    }

    [Fact]
    public void EncryptData_ShouldThrow_WhenKeyIsInvalidLength()
    {
        // Arrange
        var service = CreateService();
        byte[] plaintext = new byte[] { 1, 2, 3 };
        byte[] invalidKey = new byte[16];

        // Act
        Action act = () => service.EncryptData(plaintext, invalidKey);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*32 bytes*");
    }

    [Fact]
    public void DecryptData_ShouldThrow_WhenKeyIsInvalidLength()
    {
        // Arrange
        var service = CreateService();
        byte[] plaintext = new byte[] { 1, 2, 3 };
        byte[] encrypted = service.EncryptData(plaintext);
        byte[] invalidKey = new byte[16];

        // Act
        Action act = () => service.DecryptData(encrypted, invalidKey);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
           .WithMessage("*32 bytes*");
    }

    [Fact]
    public void DecryptData_ShouldThrow_WhenEncryptedDataIsTooShort()
    {
        // Arrange
        var service = CreateService();
        byte[] invalidData = new byte[10]; // < nonce + tag

        // Act
        Action act = () => service.DecryptData(invalidData);

        // Assert
        act.ShouldThrow<ArgumentException>()
           .WithMessage("*Invalid encrypted data*");
    }
}
