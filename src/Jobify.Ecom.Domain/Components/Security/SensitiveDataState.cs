namespace Jobify.Ecom.Domain.Components.Security;

public class SensitiveDataState<TSensitive>
    where TSensitive : ISensitiveData
{
    private SensitiveDataState() { }

    public byte[] EncryptedData { get; private set; } = [];
    public TSensitive? SensitiveData { get; private set; } = default;

    public void SetSensitiveData(TSensitive data) => SensitiveData = data;
    public void ClearSensitiveData() => SensitiveData = default;

    internal void SetEncryptedData(byte[] data) => EncryptedData = data;
    public void ClearEncryptedData() => EncryptedData = [];
}
