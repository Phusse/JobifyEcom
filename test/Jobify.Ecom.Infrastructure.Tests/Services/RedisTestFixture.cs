using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Jobify.Ecom.Infrastructure.Tests.Services;

public class RedisTestFixture : IAsyncLifetime
{
    private readonly IContainer _redisContainer;

    public string ConnectionString => $"{_redisContainer.Hostname}:{_redisContainer.GetMappedPublicPort(6379)}";

    public RedisTestFixture()
    {
        _redisContainer = new ContainerBuilder("redis:latest")
            .WithPortBinding(6379, true)
            // .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
    }

    public async Task InitializeAsync()
        => await _redisContainer.StartAsync();

    public async Task DisposeAsync()
        => await _redisContainer.DisposeAsync();
}

internal sealed class TestObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}