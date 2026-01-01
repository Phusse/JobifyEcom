namespace Jobify.Ecom.Application.Services;

public interface IDataEncryptionService
{
	byte[] EncryptData(byte[] data);
	byte[] DecryptData(byte[] encryptedData);

	byte[] EncryptData(byte[] data, byte[] key);
	byte[] DecryptData(byte[] encryptedData, byte[] key);
}
