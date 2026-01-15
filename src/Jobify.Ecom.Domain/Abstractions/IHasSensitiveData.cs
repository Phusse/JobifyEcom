using Jobify.Ecom.Domain.Components.Security;

namespace Jobify.Ecom.Domain.Abstractions;

internal interface IHasSensitiveData<TSensitive> where TSensitive : ISensitiveData
{
    byte[] EncryptedData { get; }
    TSensitive? SensitiveData { get; }

    void SetEncryptedData(byte[] data);

    void SetSensitiveData(TSensitive data);
    void ClearSensitiveData();
}
