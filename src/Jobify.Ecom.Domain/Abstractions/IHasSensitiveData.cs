using Jobify.Ecom.Domain.Components.Security;

namespace Jobify.Ecom.Domain.Abstractions;

internal interface IHasSensitiveData<TSensitive> where TSensitive : ISensitiveData
{
    byte[] EncryptedData { get; }
    TSensitive? SensitiveData { get; }

    void SetSensitiveData(TSensitive data);
    void ClearSensitiveData();

    void SetEncryptedData(byte[] data);
    void ClearEncryptedData();
}
