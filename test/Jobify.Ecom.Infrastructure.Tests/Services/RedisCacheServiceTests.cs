using Jobify.Ecom.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jobify.Ecom.Infrastructure.Tests.Services;

[Collection("Redis collection")]
public class RedisCacheServiceTests
{
    private readonly RedisCacheService _cache;

    public RedisCacheServiceTests(RedisTestFixture fixture)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = fixture.ConnectionString
            })
            .Build();

        _cache = new RedisCacheService(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );
    }

    [Fact]
    public async Task SetAndGet_ShouldReturnStoredValue()
    {
        var key = "test:key:1";
        var value = new TestObject { Id = 1, Name = "Victor" };

        var setResult = await _cache.SetAsync(key, value, TimeSpan.FromMinutes(1));
        var result = await _cache.GetAsync<TestObject>(key);

        Assert.True(setResult);
        Assert.NotNull(result);
        Assert.Equal(value.Id, result!.Id);
        Assert.Equal(value.Name, result.Name);
    }

    [Fact]
    public async Task Exists_ShouldReturnTrue_WhenKeyExists()
    {
        var key = "test:key:exists";

        await _cache.SetAsync(key, "hello");

        var exists = await _cache.ExistsAsync(key);

        Assert.True(exists);
    }

    [Fact]
    public async Task Remove_ShouldDeleteKey()
    {
        var key = "test:key:remove";

        await _cache.SetAsync(key, "value");
        var removed = await _cache.RemoveAsync(key);
        var exists = await _cache.ExistsAsync(key);

        Assert.True(removed);
        Assert.False(exists);
    }

    [Fact]
    public async Task Get_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        var result = await _cache.GetAsync<string>("non-existent-key");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SetAsync_ShouldReturnFalse_WhenKeyIsInvalid(string key)
    {
        var result = await _cache.SetAsync(key, "value");

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyIsInvalid(string key)
    {
        var result = await _cache.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ShouldExpireKey_WhenTTLIsProvided()
    {
        var key = $"ttl:{Guid.NewGuid()}";

        await _cache.SetAsync(key, "temp", TimeSpan.FromMilliseconds(300));

        await Task.Delay(500);

        var exists = await _cache.ExistsAsync(key);

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        var key = $"missing:{Guid.NewGuid()}";

        var exists = await _cache.ExistsAsync(key);

        Assert.False(exists);
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        var key = $"missing:{Guid.NewGuid()}";

        var removed = await _cache.RemoveAsync(key);

        Assert.False(removed);
    }

    private sealed class NonSerializable
    {
        public NonSerializable Self => this;
    }

    [Fact]
    public async Task SetAsync_ShouldReturnFalse_WhenSerializationFails()
    {
        var key = $"bad:{Guid.NewGuid()}";
        var value = new NonSerializable();

        var result = await _cache.SetAsync(key, value);

        Assert.False(result);
    }

    [Fact]
    public async Task Service_ShouldGracefullyDisable_WhenRedisConnectionStringIsMissing()
    {
        var configuration = new ConfigurationBuilder().Build();

        var service = new RedisCacheService(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );

        var result = await service.SetAsync("key", "value");

        Assert.False(result);
    }

    [Fact]
    public async Task Service_ShouldNotThrow_WhenRedisIsUnreachable()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = "127.0.0.1:1" // guaranteed invalid
            })
            .Build();

        var service = new RedisCacheService(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );

        var result = await service.GetAsync<string>("key");

        Assert.Null(result);
    }
    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyIsNullOrEmpty()
    {
        var service = new RedisCacheService(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.ExistsAsync(null));
        Assert.False(await service.ExistsAsync(""));
        Assert.False(await service.ExistsAsync("   "));
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenDbIsNull()
    {
        var service = new RedisCacheService(
            new ConfigurationBuilder().Build(), // no Redis connection string
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.ExistsAsync("any-key"));
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenKeyIsNullOrEmpty()
    {
        var service = new RedisCacheService(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.RemoveAsync(null));
        Assert.False(await service.RemoveAsync(""));
        Assert.False(await service.RemoveAsync("   "));
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenDbIsNull()
    {
        var service = new RedisCacheService(
            new ConfigurationBuilder().Build(), // no Redis connection string
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.RemoveAsync("any-key"));
    }
}
