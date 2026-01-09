using System.Security.Cryptography;
using System.Text;
using Jobify.Application.Enums;
using Jobify.Application.Services;
using Jobify.Infrastructure.Configurations.Security;
using Microsoft.Extensions.Options;

namespace Jobify.Infrastructure.Services;

internal class AesGcmDataEncryptionService : IDataEncryptionService
{
    private readonly Dictionary<byte, byte[]> _masterKeys;
    private readonly byte _currentVersion;

    private const int KeySizeBytes = 32;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    public AesGcmDataEncryptionService(IOptions<DataEncryptionOptions> options)
    {
        if (options?.Value?.Keys is null || options.Value.Keys.Count is 0)
            throw new InvalidOperationException("At least one key must be configured.");

        _currentVersion = options.Value.CurrentKeyVersion;
        _masterKeys = options.Value.Keys.ToDictionary(
            kvp => kvp.Key,
            kvp => Convert.FromBase64String(kvp.Value)
        );

        foreach (var key in _masterKeys.Values)
        {
            if (key.Length != KeySizeBytes)
                throw new InvalidOperationException($"Each key must be {KeySizeBytes} bytes (AES-256).");
        }
    }

    public byte[] Encrypt(byte[] data, CryptoPurpose purpose)
    {
        byte[] masterKey = _masterKeys[_currentVersion];
        byte[] derivedKey = DeriveKey(masterKey, purpose);

        byte[] aad = [_currentVersion, (byte)purpose];
        byte[] encrypted = EncryptInternal(data, derivedKey, aad);

        byte[] result = new byte[1 + encrypted.Length];
        result[0] = _currentVersion;
        encrypted.CopyTo(result.AsSpan(1));

        return result;
    }

    public byte[] Decrypt(byte[] encryptedData, CryptoPurpose purpose)
    {
        if (encryptedData.Length < 1)
            throw new ArgumentException("Invalid encrypted data.");

        byte version = encryptedData[0];

        if (!_masterKeys.TryGetValue(version, out byte[]? masterKey))
            throw new InvalidOperationException($"No key configured for version {version}");

        byte[] derivedKey = DeriveKey(masterKey, purpose);
        byte[] aad = [version, (byte)purpose];

        ReadOnlySpan<byte> payload = encryptedData.AsSpan(1);
        return DecryptInternal(payload, derivedKey, aad);
    }

    private static byte[] DeriveKey(byte[] masterKey, CryptoPurpose purpose)
    {
        return HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            masterKey,
            KeySizeBytes,
            info: Encoding.UTF8.GetBytes(purpose.ToString())
        );
    }

    private static byte[] EncryptInternal(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key, ReadOnlySpan<byte> aad)
    {
        byte[] nonce = new byte[NonceSizeBytes];
        RandomNumberGenerator.Fill(nonce);

        byte[] result = new byte[NonceSizeBytes + TagSizeBytes + data.Length];
        Span<byte> nonceSpan = result.AsSpan(0, NonceSizeBytes);
        Span<byte> tagSpan = result.AsSpan(NonceSizeBytes, TagSizeBytes);
        Span<byte> cipherSpan = result.AsSpan(NonceSizeBytes + TagSizeBytes);

        nonce.CopyTo(nonceSpan);

        using AesGcm aesGcm = new(key, TagSizeBytes);
        aesGcm.Encrypt(nonceSpan, data, cipherSpan, tagSpan, aad);

        return result;
    }

    private static byte[] DecryptInternal(ReadOnlySpan<byte> encryptedData, ReadOnlySpan<byte> key, ReadOnlySpan<byte> aad)
    {
        if (encryptedData.Length < NonceSizeBytes + TagSizeBytes)
            throw new ArgumentException("Invalid encrypted data.");

        ReadOnlySpan<byte> nonce = encryptedData[..NonceSizeBytes];
        ReadOnlySpan<byte> tag = encryptedData.Slice(NonceSizeBytes, TagSizeBytes);
        ReadOnlySpan<byte> cipher = encryptedData[(NonceSizeBytes + TagSizeBytes)..];

        byte[] plaintext = new byte[cipher.Length];
        using AesGcm aesGcm = new(key, TagSizeBytes);
        aesGcm.Decrypt(nonce, cipher, tag, plaintext, aad);

        return plaintext;
    }
}
