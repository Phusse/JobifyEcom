namespace Jobify.Ecom.Domain.Components.Security;

public class SensitiveDataState<TSensitive>
    where TSensitive : ISensitiveData
{
    public byte[] EncryptedData { get; private set; } = [];
    public TSensitive? SensitiveData { get; private set; } = default;

    internal void SetEncryptedData(byte[] data) => EncryptedData = data;

    internal void SetSensitiveData(TSensitive data) => SensitiveData = data;
    internal void ClearSensitiveData() => SensitiveData = default;
}
