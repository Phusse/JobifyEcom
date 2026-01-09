using FluentAssertions;
using Jobify.Domain.Components.Security;

namespace Jobify.Domain.Tests.Components.Security;

public class FakeSensitiveData : ISensitiveData { }

public class SensitiveDataStateTests
{
    [Fact]
    public void Constructor_Should_HaveEmptyEncryptedDataAndNullSensitiveData()
    {
        SensitiveDataState<FakeSensitiveData> state = new();

        state.EncryptedData.Should().BeEmpty();
        state.SensitiveData.Should().BeNull();
    }

    [Fact]
    public void SetEncryptedData_Should_UpdateEncryptedData()
    {
        SensitiveDataState<FakeSensitiveData> state = new();
        byte[] data = [1, 2, 3];

        state.SetEncryptedData(data);

        state.EncryptedData.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void SetSensitiveData_Should_UpdateSensitiveData()
    {
        SensitiveDataState<FakeSensitiveData> state = new();
        FakeSensitiveData sensitive = new();

        state.SetSensitiveData(sensitive);

        state.SensitiveData.Should().Be(sensitive);
    }

    [Fact]
    public void ClearSensitiveData_Should_ResetSensitiveDataToNull()
    {
        SensitiveDataState<FakeSensitiveData> state = new();
        state.SetSensitiveData(new FakeSensitiveData());

        state.ClearSensitiveData();

        state.SensitiveData.Should().BeNull();
    }
}
