using Jobify.Application.Enums;

namespace Jobify.Application.Services;

public interface IDataEncryptionService
{
    byte[] Encrypt(byte[] data, CryptoPurpose purpose);
    byte[] Decrypt(byte[] encryptedData, CryptoPurpose purpose);
}
