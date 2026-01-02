using System.Security.Cryptography;
using Jobify.Ecom.Application.Services;
using Jobify.Ecom.Infrastructure.Configurations.Security;
using Microsoft.Extensions.Options;

namespace Jobify.Ecom.Infrastructure.Services;

internal class AesGcmDataEncryptionService : IDataEncryptionService
{
    private readonly byte[] _key;

    private const int KeySizeBytes = 32;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    public AesGcmDataEncryptionService(IOptions<DataEncryptionOptions> options)
    {
        string base64Key = options.Value.Key;

        if (string.IsNullOrWhiteSpace(base64Key))
            throw new InvalidOperationException("DataEncryption:Key must be provided in configuration.");

        try
        {
            _key = Convert.FromBase64String(base64Key);
        }
        catch
        {
            throw new InvalidOperationException("DataEncryption:Key must be a valid Base64-encoded key.");
        }

        if (_key.Length is not KeySizeBytes)
            throw new InvalidOperationException($"DataEncryption:Key must decode to {KeySizeBytes} bytes (AES-256).");
    }

    public byte[] EncryptData(byte[] data) => EncryptData(data, _key);
    public byte[] DecryptData(byte[] encryptedData) => DecryptData(encryptedData, _key);

    public byte[] EncryptData(byte[] data, byte[] key)
    {
        if (key.Length is not KeySizeBytes)
            throw new InvalidOperationException($"Key must be {KeySizeBytes} bytes for AES-256.");

        byte[] nonce = new byte[NonceSizeBytes];
        RandomNumberGenerator.Fill(nonce);

        byte[] cipher = new byte[data.Length];
        byte[] tag = new byte[TagSizeBytes];

        using AesGcm aesGcm = new(key, tagSizeInBytes: TagSizeBytes);
        aesGcm.Encrypt(nonce, data, cipher, tag);

        byte[] result = new byte[NonceSizeBytes + TagSizeBytes + cipher.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSizeBytes);
        Buffer.BlockCopy(tag, 0, result, NonceSizeBytes, TagSizeBytes);
        Buffer.BlockCopy(cipher, 0, result, NonceSizeBytes + TagSizeBytes, cipher.Length);

        return result;
    }

    public byte[] DecryptData(byte[] encryptedData, byte[] key)
    {
        if (key.Length is not KeySizeBytes)
            throw new InvalidOperationException($"Key must be {KeySizeBytes} bytes for AES-256.");

        if (encryptedData.Length < NonceSizeBytes + TagSizeBytes)
            throw new ArgumentException("Invalid encrypted data.");

        byte[] nonce = new byte[NonceSizeBytes];
        byte[] tag = new byte[TagSizeBytes];
        byte[] cipher = new byte[encryptedData.Length - NonceSizeBytes - TagSizeBytes];

        Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSizeBytes);
        Buffer.BlockCopy(encryptedData, NonceSizeBytes, tag, 0, TagSizeBytes);
        Buffer.BlockCopy(encryptedData, NonceSizeBytes + TagSizeBytes, cipher, 0, cipher.Length);

        byte[] plaintext = new byte[cipher.Length];

        using AesGcm aesGcm = new(key, tagSizeInBytes: TagSizeBytes);
        aesGcm.Decrypt(nonce, cipher, tag, plaintext);

        return plaintext;
    }
}
